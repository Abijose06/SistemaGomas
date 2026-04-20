using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    // Usamos RoutePrefix para que coincida exactamente con la URL que espera la Caja
    [RoutePrefix("api/Facturacion")]
    public class FacturacionController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        // POST: api/Facturacion/CrearFactura
        [HttpPost]
        [Route("CrearFactura")]
        public async Task<IHttpActionResult> CrearFactura([FromBody] object factura)
        {
            using (HttpClient client = new HttpClient())
            {
                // Aquí cambiamos GetAsync por PostAsJsonAsync porque estamos enviando datos
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "Facturacion/CrearFactura", factura);

                if (response.IsSuccessStatusCode)
                {
                    // Si el CORE guardó la factura con éxito, le devolvemos la respuesta a la Caja
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }

                // Si falló (ej. no hay stock), devolvemos el error tal cual
                return StatusCode(response.StatusCode);
            }
        }
    }
}