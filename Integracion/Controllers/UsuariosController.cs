using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Integracion.Controllers
{
    [RoutePrefix("api/Usuarios")]
    public class UsuariosController : ApiController
    {
        private readonly string urlCore = ConfigurationManager.AppSettings["UrlCore"];

        // POST: api/Usuarios/Login
        [HttpPost]
        [Route("Login")]
        public async Task<IHttpActionResult> Login([FromBody] object credenciales)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "Usuarios/Login", credenciales);

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