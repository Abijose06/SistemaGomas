using System;
using System.Linq;
using System.Web.Http;
using Core.Models;

namespace Core.Controllers
{
    [RoutePrefix("api/sucursales")]
    public class SucursalesController : ApiController
    {
        private GomasContext db = new GomasContext();

        [HttpGet]
        [Route("lista")]
        public IHttpActionResult GetSucursales()
        {
            try
            {
                var sucursales = db.Database.SqlQuery<SucursalDTO>(
                    "SELECT IdSucursal, Direccion, Telefono FROM tblSucursal WHERE Estado = 1").ToList();

                return Ok(sucursales);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception("Error al cargar sucursales.", ex));
            }
        }

        public class SucursalDTO
        {
            public int IdSucursal { get; set; }
            public string Direccion { get; set; }
            public string Telefono { get; set; }
        }
    }
}