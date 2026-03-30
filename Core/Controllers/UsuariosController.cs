using System;
using System.Linq;
using System.Web.Http;
using Core.Models;
using Core.Helpers;

namespace Core.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        private GomasContext db = new GomasContext();
        // 1. Declaración del motor de logs para este controlador
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("registro")]
        public IHttpActionResult RegistrarUsuario(RegistroRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Documento))
                return BadRequest("Datos incompletos.");

            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    bool existe = db.Usuarios.Any(u => u.Documento == request.Documento);
                    if (existe)
                    {
                        log.Warn($"Intento de registro duplicado para el documento: {request.Documento}");
                        return BadRequest("Ya existe un usuario registrado con este documento.");
                    }

                    // 1. Crear el Usuario base
                    Usuario nuevoUsuario = new Usuario
                    {
                        TipoDocumento = request.TipoDocumento,
                        Documento = request.Documento,
                        Nombres = request.Nombres,
                        Apellidos = request.Apellidos,
                        Telefono = request.Telefono,
                        Correo = request.Correo,
                        ClaveHash = SeguridadHelper.HashPassword(request.ClaveHash),
                        Rol = request.Rol,
                        Estado = true
                    };

                    db.Usuarios.Add(nuevoUsuario);
                    db.SaveChanges(); // Guarda y genera el IdUsuario

                    string rol = request.Rol.ToLower();

                    // 2. Inteligencia de Creación de Perfil (Cliente vs Empleado)
                    if (rol == "cliente" || rol == "clienteweb")
                    {
                        db.Database.ExecuteSqlCommand("INSERT INTO tblCliente (IdUsuario, Estado) VALUES (@p0, 1)", nuevoUsuario.IdUsuario);
                        log.Info($"Perfil de Cliente creado para el Usuario ID: {nuevoUsuario.IdUsuario}");
                    }
                    else if (rol == "cajero" || rol == "administrador")
                    {
                        if (request.Sueldo == null || request.IdSucursal == null)
                        {
                            throw new Exception("Para registrar un empleado, el Sueldo y el IdSucursal son obligatorios.");
                        }

                        string sqlEmpleado = "INSERT INTO tblEmpleado (IdUsuario, Sueldo, FechaIngreso, IdSucursal, Estado) VALUES (@p0, @p1, GETDATE(), @p2, 1)";
                        db.Database.ExecuteSqlCommand(sqlEmpleado, nuevoUsuario.IdUsuario, request.Sueldo, request.IdSucursal);
                        log.Info($"Perfil de Empleado creado para el Usuario ID: {nuevoUsuario.IdUsuario} en Sucursal {request.IdSucursal}");
                    }

                    transaccion.Commit();
                    return Ok(new { Mensaje = "Usuario registrado exitosamente.", IdUsuario = nuevoUsuario.IdUsuario });
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    log.Error("Error crítico al intentar registrar un nuevo usuario.", ex);
                    return InternalServerError(new Exception("Error al registrar el usuario: " + ex.Message));
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Documento) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Debe enviar el documento y la contraseña.");

            try
            {
                var usuarioDb = db.Usuarios.FirstOrDefault(u => u.Documento == request.Documento);

                if (usuarioDb == null)
                {
                    log.Warn($"Intento de login con documento inexistente: {request.Documento}");
                    return NotFound();
                }

                if (!usuarioDb.Estado)
                {
                    log.Warn($"Intento de login a cuenta inactiva. Documento: {request.Documento}");
                    return BadRequest("Este usuario está inactivo.");
                }

                bool claveCorrecta = SeguridadHelper.VerificarPassword(request.Password, usuarioDb.ClaveHash);

                if (!claveCorrecta)
                {
                    // Alerta de seguridad para detectar posibles ataques de fuerza bruta
                    log.Warn($"Contraseña incorrecta. Documento: {request.Documento}");
                    return Unauthorized();
                }

                log.Info($"Login exitoso. Usuario ID: {usuarioDb.IdUsuario}, Rol: {usuarioDb.Rol}");

                // --- NUEVO: Inteligencia de Roles (Cliente vs Empleado) ---
                int? idClienteReal = null;
                int? idEmpleadoReal = null;
                int? idSucursalReal = null;

                string rol = usuarioDb.Rol.ToLower();

                if (rol == "cliente" || rol == "clienteweb")
                {
                    idClienteReal = db.Database.SqlQuery<int?>(
                        "SELECT IdCliente FROM tblCliente WHERE IdUsuario = @p0", usuarioDb.IdUsuario).FirstOrDefault();
                }
                else if (rol == "cajero" || rol == "administrador")
                {
                    // Buscamos los datos del empleado y su sucursal
                    var datosEmpleado = db.Database.SqlQuery<DatosEmpleadoDTO>(
                        "SELECT IdEmpleado, IdSucursal FROM tblEmpleado WHERE IdUsuario = @p0", usuarioDb.IdUsuario).FirstOrDefault();

                    if (datosEmpleado != null)
                    {
                        idEmpleadoReal = datosEmpleado.IdEmpleado;
                        idSucursalReal = datosEmpleado.IdSucursal;
                    }
                }

                log.Info($"Login exitoso. Usuario ID: {usuarioDb.IdUsuario}, Rol: {usuarioDb.Rol}");

                return Ok(new
                {
                    IdUsuario = usuarioDb.IdUsuario,
                    IdCliente = idClienteReal,     // Se llena si es cliente
                    IdEmpleado = idEmpleadoReal,   // Se llena si es empleado
                    IdSucursal = idSucursalReal,   // Vital para que la Caja sepa de dónde descontar inventario
                    NombreCompleto = usuarioDb.Nombres + " " + usuarioDb.Apellidos,
                    Rol = usuarioDb.Rol,
                    Token = Guid.NewGuid().ToString()
                });
            }
            catch (Exception ex)
            {
                log.Error($"Error de sistema durante el intento de login del documento {request.Documento}", ex);
                return InternalServerError(new Exception("Error interno al procesar el inicio de sesión."));
            }
        }
        // Ruta: GET api/usuarios/buscar/{documento}
        [HttpGet]
        [Route("buscar/{documento}")]
        public IHttpActionResult BuscarClientePorDocumento(string documento)
        {
            try
            {
                // Hacemos un JOIN para traer los datos del usuario y su IdCliente al mismo tiempo
                string sql = @"
                    SELECT c.IdCliente, u.Nombres, u.Apellidos, u.Documento, u.Telefono 
                    FROM tblCliente c
                    INNER JOIN tblUsuario u ON c.IdUsuario = u.IdUsuario
                    WHERE u.Documento = @p0 AND u.Estado = 1 AND c.Estado = 1";

                var cliente = db.Database.SqlQuery<ClienteBuscadoDTO>(sql, documento).FirstOrDefault();

                if (cliente == null)
                {
                    return NotFound(); // El cajero sabrá que tiene que registrarlo primero
                }

                return Ok(cliente);
            }
            catch (Exception ex)
            {
                log.Error($"Error al buscar cliente con documento {documento}", ex);
                return InternalServerError(new Exception("Error al buscar el cliente."));
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class LoginRequest
    {
        public string Documento { get; set; }
        public string Password { get; set; }
    }

    public class ClienteBuscadoDTO
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Documento { get; set; }
        public string Telefono { get; set; }
    }
    public class DatosEmpleadoDTO
    {
        public int IdEmpleado { get; set; }
        public int? IdSucursal { get; set; }
    }

    public class RegistroRequest
    {
        public int TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string ClaveHash { get; set; }
        public string Rol { get; set; }

        // --- Campos exclusivos para cuando se registra un Empleado ---
        public decimal? Sueldo { get; set; }
        public int? IdSucursal { get; set; }
    }
}
