using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    [RoutePrefix("api/caja")]
    public class CajaController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        [HttpGet]
        [Route("estado-turno/{idEmpleado}")]
        public async Task<IHttpActionResult> VerificarTurno(int idEmpleado)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + $"caja/estado-turno/{idEmpleado}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        [HttpPost]
        [Route("abrir")]
        public async Task<IHttpActionResult> AbrirTurno([FromBody] object request)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "caja/abrir", request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        [HttpPost]
        [Route("cerrar")]
        public async Task<IHttpActionResult> CerrarTurno([FromBody] object request)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "caja/cerrar", request);
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