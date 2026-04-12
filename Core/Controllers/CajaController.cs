using System;
using System.Linq;
using System.Web.Http;
using Core.Models;

namespace Core.Controllers
{
    [RoutePrefix("api/caja")]
    public class CajaController : ApiController
    {
        private GomasContext db = new GomasContext();

        // 1. VERIFICAR SI EL CAJERO TIENE UN TURNO ABIERTO (La Caja lo llama al hacer Login)
        [HttpGet]
        [Route("estado-turno/{idEmpleado}")]
        public IHttpActionResult VerificarTurno(int idEmpleado)
        {
            var turnoAbierto = db.Database.SqlQuery<TurnoDTO>(@"
                SELECT TOP 1 IdTurno, FechaApertura, MontoApertura 
                FROM tblTurno_Caja 
                WHERE IdEmpleado = @p0 AND FechaCierre IS NULL",
                idEmpleado).FirstOrDefault();

            if (turnoAbierto == null)
            {
                return Ok(new { Estado = "Cerrado", Mensaje = "Debe abrir la caja antes de facturar." });
            }

            return Ok(new { Estado = "Abierto", IdTurno = turnoAbierto.IdTurno, MontoApertura = turnoAbierto.MontoApertura });
        }

        // 2. ABRIR EL TURNO DE CAJA
        [HttpPost]
        [Route("abrir")]
        public IHttpActionResult AbrirTurno(AbrirTurnoRequest request)
        {
            try
            {
                // Verificamos que no tenga ya un turno abierto
                var existeAbierto = db.Database.SqlQuery<int>(
                    "SELECT COUNT(1) FROM tblTurno_Caja WHERE IdEmpleado = @p0 AND FechaCierre IS NULL",
                    request.IdEmpleado).Single();

                if (existeAbierto > 0)
                    return BadRequest("El empleado ya tiene un turno de caja abierto.");

                db.Database.ExecuteSqlCommand(@"
                    INSERT INTO tblTurno_Caja (IdEmpleado, FechaApertura, MontoApertura) 
                    VALUES (@p0, GETDATE(), @p1)",
                    request.IdEmpleado, request.MontoApertura);

                return Ok(new { Mensaje = "Caja abierta exitosamente." });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al abrir el turno: " + ex.Message));
            }
        }

        // 3. CERRAR EL TURNO DE CAJA (Cuadre final)
        [HttpPost]
        [Route("cerrar")]
        public IHttpActionResult CerrarTurno(CerrarTurnoRequest request)
        {
            try
            {
                int filasAfectadas = db.Database.ExecuteSqlCommand(@"
                    UPDATE tblTurno_Caja 
                    SET FechaCierre = GETDATE(), MontoCierre = @p0 
                    WHERE IdTurno = @p1 AND FechaCierre IS NULL",
                    request.MontoCierre, request.IdTurno);

                if (filasAfectadas == 0)
                    return BadRequest("El turno no existe o ya fue cerrado.");

                return Ok(new { Mensaje = "Caja cerrada correctamente. Imprimiendo cuadre..." });
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al cerrar el turno: " + ex.Message));
            }
        }
    }

    // --- DTOs necesarios (Ponlos al final del archivo) ---
    public class TurnoDTO { public int IdTurno { get; set; } public DateTime FechaApertura { get; set; } public decimal MontoApertura { get; set; } }
    public class AbrirTurnoRequest { public int IdEmpleado { get; set; } public decimal MontoApertura { get; set; } }
    public class CerrarTurnoRequest { public int IdTurno { get; set; } public decimal MontoCierre { get; set; } }
}