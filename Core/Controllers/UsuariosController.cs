using Core.Helpers;
using Core.Models;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;

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
        public IHttpActionResult Registro(RegistroRequest request)
        {
            // Usamos una transacción para que si falla crear el cliente, no se quede el usuario "huérfano"
            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Hashear la clave usando el nombre correcto del método
                    string hashGenerado = SeguridadHelper.CalcularHash(request.Password);

                    // 2. Crear el registro en la tabla padre (tblUsuario)
                    var nuevoUsuario = new Usuario
                    {
                        TipoDocumento = request.TipoDocumento,
                        Documento = request.Documento,
                        Nombres = request.Nombres,
                        Apellidos = request.Apellidos,
                        Telefono = request.Telefono,
                        Correo = request.Correo,
                        Rol = request.Rol,
                        ClaveHash = hashGenerado,
                        Estado = true
                    };

                    db.Usuarios.Add(nuevoUsuario);
                    db.SaveChanges(); // Guardamos para que SQL nos devuelva el IdUsuario generado

                    // 3. Crear el registro en la tabla hija correspondiente
                    if (request.Rol == "Cliente" || request.Rol == "ClienteWeb")
                    {
                        var nuevoCliente = new Cliente
                        {
                            IdUsuario = nuevoUsuario.IdUsuario,
                            Direccion = request.Direccion,
                            Estado = true
                        };
                        db.Clientes.Add(nuevoCliente);
                    }
                    else if (request.Rol == "Cajero" || request.Rol == "Administrador")
                    {
                        var nuevoEmpleado = new Empleado
                        {
                            IdUsuario = nuevoUsuario.IdUsuario,
                            Sueldo = request.Sueldo ?? 0,
                            IdSucursal = request.IdSucursal,
                            FechaIngreso = DateTime.Now,
                            Estado = true
                        };
                        db.Empleados.Add(nuevoEmpleado);
                    }

                    db.SaveChanges();
                    transaccion.Commit(); // ¡Todo perfecto, guardar definitivamente!

                    return Ok(new { Mensaje = "Usuario registrado exitosamente", IdUsuario = nuevoUsuario.IdUsuario });
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    log.Error("Fallo al registrar usuario", ex);
                    return InternalServerError(new Exception("Error crítico al registrar el usuario."));
                }
            }
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login(LoginRequest request)
        {
            try
            {
                // 1. Buscamos el usuario por Tipo y Número de documento
                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.TipoDocumento == request.TipoDocumento &&
                    u.Documento == request.Documento &&
                    u.Estado == true);

                // 2. Si no existe o la clave (hasheada) no coincide, fuera.
                if (usuario == null || !SeguridadHelper.VerificarHash(request.Password, usuario.ClaveHash))
                {
                    return Content(HttpStatusCode.Unauthorized, "Credenciales incorrectas.");
                }

                // 3. Buscamos si es Cliente o Empleado para devolver el perfil completo
                var cliente = db.Clientes.FirstOrDefault(c => c.IdUsuario == usuario.IdUsuario);
                var empleado = db.Database.SqlQuery<EmpleadoLoginDTO>(
                    "SELECT IdEmpleado, IdSucursal FROM tblEmpleado WHERE IdUsuario = @p0",
                    usuario.IdUsuario).FirstOrDefault();

                return Ok(new
                {
                    IdUsuario = usuario.IdUsuario,
                    IdCliente = cliente?.IdCliente,
                    IdEmpleado = empleado?.IdEmpleado,
                    IdSucursal = empleado?.IdSucursal,
                    NombreCompleto = usuario.Nombres + " " + usuario.Apellidos,
                    Rol = usuario.Rol,
                    Token = "TOKEN-SIMULADO-" + Guid.NewGuid().ToString() // Aquí iría un JWT real
                });
            }
            catch (Exception ex)
            {
                log.Error("Error en Login", ex);
                return InternalServerError(new Exception("Error al procesar el inicio de sesión."));
            }
        }

        // Clase auxiliar para el query de empleado
        private class EmpleadoLoginDTO { public int IdEmpleado { get; set; } public int IdSucursal { get; set; } }

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

        // =========================================================
        // NUEVOS MÉTODOS AÑADIDOS PARA ALIMENTAR LA CAJA
        // =========================================================
        [HttpGet]
        [Route("clientes")]
        public IHttpActionResult GetClientes()
        {
            var clientes = db.Database.SqlQuery<UsuarioComboDTO>(
                "SELECT c.IdCliente as Id, u.Nombres + ' ' + u.Apellidos AS Nombre FROM tblCliente c INNER JOIN tblUsuario u ON c.IdUsuario = u.IdUsuario WHERE c.Estado = 1").ToList();
            return Ok(clientes);
        }

        [HttpGet]
        [Route("empleados")]
        public IHttpActionResult GetEmpleados()
        {
            var empleados = db.Database.SqlQuery<UsuarioComboDTO>(
                "SELECT e.IdEmpleado as Id, u.Nombres + ' ' + u.Apellidos AS Nombre FROM tblEmpleado e INNER JOIN tblUsuario u ON e.IdUsuario = u.IdUsuario WHERE e.Estado = 1").ToList();
            return Ok(empleados);
        }
        // =========================================================

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }

    public class LoginRequest
    {
        public int TipoDocumento { get; set; }
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

    // =========================================================
    // NUEVA CLASE AÑADIDA PARA LOS COMBOBOX DE LA CAJA
    // =========================================================
    public class UsuarioComboDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
    }
    // =========================================================

    public class RegistroRequest
    {
        public int TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public string Password { get; set; } // Aquí llega el texto plano ("123456")
        public string Rol { get; set; }

        // Opcionales dependiendo del rol
        public string Direccion { get; set; } // Para el cliente
        public decimal? Sueldo { get; set; }  // Para el empleado
        public int? IdSucursal { get; set; }  // Para el empleado
    }
}