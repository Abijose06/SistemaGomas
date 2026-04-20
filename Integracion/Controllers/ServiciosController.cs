using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    [RoutePrefix("api/servicios")]
    public class ServiciosController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        [HttpGet]
        [Route("catalogo")]
        public async Task<IHttpActionResult> GetCatalogo()
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + "servicios/catalogo");

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