using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net; 

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
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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


        // POST api/Usuarios/Registro
        [HttpPost]
        [Route("Registro")]
        public async Task<IHttpActionResult> Registro([FromBody] object nuevoUsuario)
        {
            using (HttpClient client = new HttpClient())
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "usuarios/registro", nuevoUsuario);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        // GET api/Usuarios/Clientes
        [HttpGet]
        [Route("Clientes")]
        public async Task<IHttpActionResult> GetClientes()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + "usuarios/clientes");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        // GET api/Usuarios/Empleados
        [HttpGet]
        [Route("Empleados")]
        public async Task<IHttpActionResult> GetEmpleados()
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + "usuarios/empleados");
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