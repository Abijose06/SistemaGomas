using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    public class ProductosController : ApiController
    {
        // 1. Leemos la URL del CORE que guardamos en el Web.config
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        // GET: api/Productos
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            // 2. Usamos HttpClient para hacer la llamada al CORE
            using (HttpClient client = new HttpClient())
            {
                // 3. Hacemos la petición GET a https://localhost:44376/api/Productos
                HttpResponseMessage response = await client.GetAsync(urlCore + "Productos");

                // 4. Si el CORE responde que todo está bien (200 OK)
                if (response.IsSuccessStatusCode)
                {
                    // Leemos el JSON tal cual como vino del CORE y se lo devolvemos al cliente
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }

                // 5. Si el CORE falla (ej. 500 Internal Server Error), le pasamos ese mismo error al cliente
                return StatusCode(response.StatusCode);
            }
        }
    }
}