using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Core.Controllers
{
    [RoutePrefix("api/vehiculos")]
    public class VehiculosController : ApiController
    {
        private GomasContext db = new GomasContext();

        // GET: api/vehiculos/buscar?placa=A123456
        [HttpGet]
        [Route("buscar")]
        public IHttpActionResult BuscarPorPlaca(string placa)
        {
            var vehiculo = db.Database.SqlQuery<VehiculoDTO>(
                "SELECT * FROM tblVehiculo WHERE Placa = @p0", placa).FirstOrDefault();

            if (vehiculo == null) return NotFound();
            return Ok(vehiculo);
        }

        // POST: api/vehiculos/registrar
        [HttpPost]
        [Route("registrar")]
        public IHttpActionResult Registrar(VehiculoRegistroRequest request)
        {
            using (var trans = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Insertar Vehículo
                    var idVehiculo = db.Database.SqlQuery<int>(@"
                    INSERT INTO tblVehiculo (Marca, Modelo, Año, Placa, Chassis) 
                    VALUES (@p0, @p1, @p2, @p3, @p4);
                    SELECT CAST(SCOPE_IDENTITY() as int);",
                        request.Marca, request.Modelo, request.Anio, request.Placa, request.Chassis).Single();

                    // 2. Vincular con el Cliente (según tu tblCliente_Vehiculo)
                    db.Database.ExecuteSqlCommand(
                        "INSERT INTO tblCliente_Vehiculo (IdCliente, IdVehiculo) VALUES (@p0, @p1)",
                        request.IdCliente, idVehiculo);

                    trans.Commit();
                    return Ok(new { IdVehiculo = idVehiculo, Mensaje = "Vehículo registrado y vinculado." });
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    return InternalServerError(ex);
                }
            }
        }

        [HttpGet]
        [Route("cliente/{idCliente}")]
        public IHttpActionResult GetVehiculosPorCliente(int idCliente)
        {
            var vehiculos = db.Database.SqlQuery<VehiculoDTO>(@"
        SELECT v.* FROM tblVehiculo v
        JOIN tblCliente_Vehiculo cv ON v.IdVehiculo = cv.IdVehiculo
        WHERE cv.IdCliente = @p0", idCliente).ToList();

            return Ok(vehiculos);
        }
    }
    public class VehiculoDTO
    {
        public int IdVehiculo { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Placa { get; set; }
        public string Chassis { get; set; }
    }

    public class VehiculoRegistroRequest
    {
        public int IdCliente { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Anio { get; set; }
        public string Placa { get; set; }
        public string Chassis { get; set; }
    }
}
