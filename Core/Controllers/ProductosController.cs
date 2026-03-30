using System;
using System.Linq;
using System.Web.Http;
using Core.Models;

namespace Core.Controllers
{
    [RoutePrefix("api/productos")]
    public class ProductosController : ApiController
    {
        private GomasContext db = new GomasContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Ruta: GET api/productos/catalogo/{idSucursal}
        [HttpGet]
        [Route("catalogo/{idSucursal}")]
        public IHttpActionResult GetCatalogoConStock(int idSucursal)
        {
            try
            {
                // Hacemos un LEFT JOIN para traer el producto y su stock en esa sucursal específica
                string sql = @"
                    SELECT p.IdProducto, p.Marca, p.Modelo, p.Medida, p.PrecioVenta, ISNULL(i.StockActual, 0) as StockActual
                    FROM tblProducto p
                    LEFT JOIN tblInventario i ON p.IdProducto = i.IdProducto AND i.IdSucursal = @p0
                    WHERE p.Estado = 1";

                var productos = db.Database.SqlQuery<ProductoConStockDTO>(sql, idSucursal).ToList();

                if (productos.Count == 0)
                {
                    log.Warn($"Catálogo consultado para la sucursal {idSucursal} pero no hay productos activos.");
                    return NotFound();
                }

                return Ok(productos);
            }
            catch (Exception ex)
            {
                log.Error("Fallo al obtener el catálogo de productos con stock.", ex);
                return InternalServerError(new Exception("No se pudo cargar el catálogo."));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }

        // Clase auxiliar para devolver el producto + stock
        public class ProductoConStockDTO
        {
            public int IdProducto { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Medida { get; set; }
            public decimal PrecioVenta { get; set; }
            public int StockActual { get; set; }
        }
    }
}