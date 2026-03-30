using System;
using System.Linq;
using System.Web.Http;
using Core.Models;

namespace Core.Controllers
{
    [RoutePrefix("api/servicios")]
    public class ServiciosController : ApiController
    {
        private GomasContext db = new GomasContext();
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [HttpGet]
        [Route("catalogo")]
        public IHttpActionResult GetCatalogo()
        {
            try
            {
                // Devolvemos la lista de servicios activos (Alineación, Balanceo, etc.)
                var servicios = db.Database.SqlQuery<ServicioDTO>(
                    "SELECT IdServicio, NombreServicio, Precio, Descripcion FROM tblServicio WHERE Estado = 1").ToList();
                return Ok(servicios);
            }
            catch (Exception ex)
            {
                log.Error("Error al consultar el catálogo de servicios.", ex);
                return InternalServerError(new Exception("No se pudieron cargar los servicios."));
            }
        }

        public class ServicioDTO
        {
            public int IdServicio { get; set; }
            public string NombreServicio { get; set; }
            public decimal Precio { get; set; }
            public string Descripcion { get; set; }
        }
    }
}