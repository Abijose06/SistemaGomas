using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    [RoutePrefix("api/inventario")]
    public class InventarioController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        [HttpPost]
        [Route("entrada")]
        public async Task<IHttpActionResult> RegistrarEntrada([FromBody] object request)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "inventario/entrada", request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        [HttpGet]
        [Route("movimientos/{idProducto}/{idSucursal}")]
        public async Task<IHttpActionResult> GetMovimientos(int idProducto, int idSucursal)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + $"inventario/movimientos/{idProducto}/{idSucursal}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }
    }
}