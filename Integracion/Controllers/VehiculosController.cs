using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    [RoutePrefix("api/vehiculos")]
    public class VehiculosController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        [HttpGet]
        [Route("buscar")]
        public async Task<IHttpActionResult> BuscarPorPlaca(string placa)
        {
            using (HttpClient client = new HttpClient())
            {
                // Concatenamos la variable 'placa' a la URL
                HttpResponseMessage response = await client.GetAsync(urlCore + "vehiculos/buscar?placa=" + placa);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        [HttpPost]
        [Route("registrar")]
        public async Task<IHttpActionResult> Registrar([FromBody] object request)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "vehiculos/registrar", request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        [HttpGet]
        [Route("cliente/{idCliente}")]
        public async Task<IHttpActionResult> GetVehiculosPorCliente(int idCliente)
        {
            using (HttpClient client = new HttpClient())
            {
                // Usamos interpolación de strings para pasar el parámetro de ruta
                HttpResponseMessage response = await client.GetAsync(urlCore + $"vehiculos/cliente/{idCliente}");

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