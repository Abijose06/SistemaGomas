using Core.Models;
using System;
using System.Linq;
using System.Web.Http;

namespace Core.Controllers
{
    [RoutePrefix("api/inventario")]
    public class InventarioController : ApiController
    {
        private GomasContext db = new GomasContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Ruta: POST api/inventario/entrada
        [HttpPost]
        [Route("entrada")]
        public IHttpActionResult RegistrarEntrada(EntradaInventarioDTO request)
        {
            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    // 1. Sumar al stock de la sucursal
                    string sqlInventario = "UPDATE tblInventario SET StockActual = StockActual + @p0 WHERE IdProducto = @p1 AND IdSucursal = @p2";
                    int filasAfectadas = db.Database.ExecuteSqlCommand(sqlInventario, request.Cantidad, request.IdProducto, request.IdSucursal);

                    // Si no había registro en esa sucursal, lo creamos
                    if (filasAfectadas == 0)
                    {
                        db.Database.ExecuteSqlCommand(
                            "INSERT INTO tblInventario (StockActual, PuntoReorden, IdSucursal, IdProducto, Estado) VALUES (@p0, 5, @p1, @p2, 1)",
                            request.Cantidad, request.IdSucursal, request.IdProducto);
                    }

                    string sqlLog = @"
                        INSERT INTO tblMovimiento_Inventario (TipoMovimiento, Concepto, Cantidad, FechaHoraMovimiento, IdSucursal, IdProducto, Estado) 
                        VALUES ('E', 'Compra a Suplidor', @p0, GETDATE(), @p1, @p2, 1)";
                    db.Database.ExecuteSqlCommand(sqlLog, request.Cantidad, request.IdSucursal, request.IdProducto);

                    transaccion.Commit();
                    log.Info($"Entrada de mercancía registrada. Producto: {request.IdProducto}, Cantidad: {request.Cantidad}, Sucursal: {request.IdSucursal}");

                    return Ok("Inventario actualizado correctamente.");
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    log.Error("Error al ingresar mercancía al inventario.", ex);
                    return InternalServerError(new Exception("Error al actualizar el inventario."));
                }
            }


        }

        [HttpGet]
        [Route("movimientos/{idProducto}/{idSucursal}")]
        public IHttpActionResult GetMovimientos(int idProducto, int idSucursal)
        {
            var movimientos = db.Database.SqlQuery<KardexDTO>(@"
        SELECT Fecha, TipoMovimiento, Cantidad, SaldoInicial, SaldoFinal, Observacion
        FROM tblKardex_Inventario
        WHERE IdProducto = @p0 AND IdSucursal = @p1
        ORDER BY Fecha DESC", idProducto, idSucursal).ToList();

            return Ok(movimientos);
        }

        public class KardexDTO
        {
            public DateTime Fecha { get; set; }
            public string TipoMovimiento { get; set; } // 'E' o 'S'
            public int Cantidad { get; set; }
            public int SaldoInicial { get; set; }
            public int SaldoFinal { get; set; }
            public string Observacion { get; set; }
        }

        public class EntradaInventarioDTO
        {
            public int IdProducto { get; set; }
            public int IdSucursal { get; set; }
            public int Cantidad { get; set; }
        }
    }
}