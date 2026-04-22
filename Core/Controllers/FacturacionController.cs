using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Core.Models;

namespace Core.Controllers
{
    [RoutePrefix("api/facturacion")]
    public class FacturacionController : ApiController
    {
        private GomasContext db = new GomasContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpPost]
        [Route("procesar")]
        public IHttpActionResult ProcesarVenta(VentaRequest request)
        {
            if (request == null || request.Detalles == null || !request.Detalles.Any())
                return BadRequest("La factura debe tener al menos un artículo.");

            using (var transaccion = db.Database.BeginTransaction())
            {
                try
                {
                    DateTime fechaFactura = request.FechaOriginal ?? DateTime.Now;

                    string sqlFactura = @"
                        INSERT INTO tblFactura (Fecha, Impuesto, EstadoFactura, IdCliente, IdEmpleado, IdSucursal, MetodoPago, Estado) 
                        OUTPUT INSERTED.IdFactura 
                        VALUES (@p0, @p1, 'Pagada', @p2, @p3, @p4, @p5, 1)";

                    int idFactura = db.Database.SqlQuery<int>(sqlFactura,
                        fechaFactura,
                        18.00m,
                        request.IdCliente,
                        request.IdEmpleado ?? (object)DBNull.Value,
                        request.IdSucursal,
                        request.MetodoPago ?? "Efectivo").Single(); // Por defecto Efectivo si no lo mandan

                    // 2. Procesar los Detalles (Inteligencia de P vs S)
                    foreach (var item in request.Detalles)
                    {
                        if (item.TipoItem == "P" && item.IdProducto.HasValue)
                        {
                            // ES UN PRODUCTO: Validamos y descontamos stock
                            int stockDisponible = db.Database.SqlQuery<int>(
                                "SELECT StockActual FROM tblInventario WHERE IdProducto = @p0 AND IdSucursal = @p1",
                                item.IdProducto, request.IdSucursal).FirstOrDefault();

                            if (stockDisponible < item.Cantidad)
                                throw new Exception($"Stock insuficiente para el Producto ID {item.IdProducto}. Disponible: {stockDisponible}");

                            string sqlDetalleP = @"
                                INSERT INTO tblDetalle_Factura (TipoItem, IdProducto, Cantidad, PrecioUnitario, IdFactura, IdVehiculo, Estado) 
                                VALUES ('P', @p0, @p1, @p2, @p3, @p4, 1)";
                            db.Database.ExecuteSqlCommand(sqlDetalleP, item.IdProducto, item.Cantidad, item.PrecioUnitario, idFactura, request.IdVehiculo);

                            string sqlInventario = "UPDATE tblInventario SET StockActual = StockActual - @p0 WHERE IdProducto = @p1 AND IdSucursal = @p2";
                            db.Database.ExecuteSqlCommand(sqlInventario, item.Cantidad, item.IdProducto, request.IdSucursal);

                            string sqlLog = @"
                                INSERT INTO tblMovimiento_Inventario (TipoMovimiento, Concepto, Cantidad, FechaHoraMovimiento, IdFactura, IdSucursal, IdProducto, Estado) 
                                VALUES ('S', 'Venta Facturación', @p0, GETDATE(), @p1, @p2, @p3, 1)";
                            db.Database.ExecuteSqlCommand(sqlLog, item.Cantidad, idFactura, request.IdSucursal, item.IdProducto);
                        }
                        else if (item.TipoItem == "S" && item.IdServicio.HasValue)
                        {
                            // ES UN SERVICIO: Solo se cobra, NO afecta inventario
                            string sqlDetalleS = @"
                                INSERT INTO tblDetalle_Factura (TipoItem, IdServicio, Cantidad, PrecioUnitario, IdFactura, IdVehiculo, Estado) 
                                VALUES ('S', @p0, @p1, @p2, @p3, @p4, 1)";
                            db.Database.ExecuteSqlCommand(sqlDetalleS, item.IdServicio, item.Cantidad, item.PrecioUnitario, idFactura, request.IdVehiculo);
                        }
                        else
                        {
                            throw new Exception("Formato de ítem incorrecto. Verifique TipoItem ('P' o 'S') y su ID correspondiente.");
                        }
                    }

                    transaccion.Commit();

                    log.Info($"Venta procesada correctamente. Factura ID: {idFactura}, Cliente ID: {request.IdCliente}, Sucursal: {request.IdSucursal}. Se afectó el inventario.");

                    return Ok(new { Mensaje = "Venta procesada exitosamente.", NumeroFactura = idFactura });
                }
                catch (Exception ex)
                {
                    transaccion.Rollback();
                    log.Error($"Fallo crítico y Rollback ejecutado al intentar procesar venta para el Cliente ID: {request.IdCliente}", ex);
                    return InternalServerError(new Exception("Error crítico al procesar la venta. Se ha revertido la operación.")); 
                }
            }
        }
        // Ruta: GET api/facturacion/resumen-diario/{idSucursal}
        [HttpGet]
        [Route("resumen-diario/{idSucursal}")]
        public IHttpActionResult GetResumenDiario(int idSucursal)
        {
            try
            {
                // Buscamos todas las facturas de hoy en esa sucursal
                string sql = @"
                    SELECT IdFactura, Fecha, TotalGeneral, MetodoPago, EstadoFactura 
                    FROM tblFactura 
                    WHERE IdSucursal = @p0 
                    AND CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE) 
                    AND Estado = 1";

                var resumen = db.Database.SqlQuery<ResumenDiarioDTO>(sql, idSucursal).ToList();

                if (resumen.Count == 0)
                {
                    return Ok(new { Mensaje = "No hay ventas registradas hoy en esta sucursal.", TotalVendido = 0 });
                }

                return Ok(resumen);
            }
            catch (Exception ex)
            {
                log.Error($"Error al obtener el resumen diario de la sucursal {idSucursal}", ex);
                return InternalServerError(new Exception("No se pudo obtener el resumen de ventas del día."));
            }
        }

        // Agrega esta clase auxiliar al final del archivo FacturacionController.cs
        public class ResumenDiarioDTO
        {
            public int IdFactura { get; set; }
            public DateTime Fecha { get; set; }
            public decimal TotalGeneral { get; set; }
            public string MetodoPago { get; set; }
            public string EstadoFactura { get; set; }
        }
        [HttpGet]
        [Route("historial/{idCliente}")]
        public IHttpActionResult GetHistorial(int idCliente)
        {
            try
            {
                var facturas = db.Database.SqlQuery<FacturaHistorialDTO>(
                    @"SELECT f.IdFactura, f.Fecha, f.TotalGeneral, f.EstadoFactura 
                      FROM tblFactura f WHERE f.IdCliente = @p0", idCliente).ToList();

                if (facturas.Count == 0) return NotFound();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                log.Error($"Error al consultar el historial de facturas del cliente {idCliente}", ex);
                return InternalServerError(new Exception("No se pudo obtener el historial de facturas."));
            }
        }
        // Ruta: GET api/facturacion/detalle/{idFactura}
        [HttpGet]
        [Route("detalle/{idFactura}")]
        public IHttpActionResult GetDetalleFactura(int idFactura)
        {
            try
            {
                // Hacemos un JOIN rápido para devolverle el nombre de la goma y no solo el ID
                string sql = @"
                    SELECT p.Marca, p.Modelo, p.Medida, d.Cantidad, d.PrecioUnitario, (d.Cantidad * d.PrecioUnitario) as SubTotal
                    FROM tblDetalle_Factura d
                    INNER JOIN tblProducto p ON d.IdProducto = p.IdProducto
                    WHERE d.IdFactura = @p0";

                var detalles = db.Database.SqlQuery<FacturaDetalleRespuestaDTO>(sql, idFactura).ToList();

                if (detalles.Count == 0) return NotFound();

                return Ok(detalles);
            }
            catch (Exception ex)
            {
                log.Error($"Error al consultar el detalle de la factura {idFactura}", ex);
                return InternalServerError(new Exception("No se pudo obtener el detalle de la factura."));
            }
        }


        public class FacturaHistorialDTO
        {
            public int IdFactura { get; set; }
            public DateTime Fecha { get; set; }
            public decimal TotalGeneral { get; set; }
            public string EstadoFactura { get; set; }
        }

        

        [HttpGet]
        [Route("resumen/{idSucursal}")]
        public IHttpActionResult ResumenDiario(int idSucursal)
        {
            // Obtiene las ventas solo de HOY para la sucursal específica
            var resumen = db.Database.SqlQuery<FacturaHistorialDTO>(@"
        SELECT IdFactura, Fecha, TotalGeneral, MetodoPago as EstadoFactura
        FROM tblFactura 
        WHERE IdSucursal = @p0 AND CAST(Fecha AS DATE) = CAST(GETDATE() AS DATE) 
        AND EstadoFactura = 'Activa'",
                idSucursal).ToList();

            return Ok(resumen);
        }

    }

    
    public class VentaRequest
    {
        public DateTime? FechaOriginal { get; set; }
        public int IdCliente { get; set; }
        public int? IdEmpleado { get; set; }
        public int IdSucursal { get; set; }
        public int IdVehiculo { get; set; }
        public string MetodoPago { get; set; } // NUEVO: Efectivo, Tarjeta o Transferencia
        public List<VentaDetalleDTO> Detalles { get; set; }
    }

    public class VentaDetalleDTO
    {
        public string TipoItem { get; set; } // 'P' para Producto, 'S' para Servicio
        public int? IdProducto { get; set; }
        public int? IdServicio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class FacturaDetalleRespuestaDTO
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Medida { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }

    public class DetalleItemDTO
    {
        public string TipoItem { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string NombreServicio { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
    }
}