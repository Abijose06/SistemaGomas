using System;
using System.Configuration;
using System.Net;
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
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
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


        // GET api/Facturacion/Historial/{idCliente}
        [HttpGet]
        [Route("Historial/{idCliente}")]
        public async Task<IHttpActionResult> Historial(int idCliente)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + $"facturacion/historial/{idCliente}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        // GET api/Facturacion/Detalle/{idFactura}
        [HttpGet]
        [Route("Detalle/{idFactura}")]
        public async Task<IHttpActionResult> Detalle(int idFactura)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(urlCore + $"facturacion/detalle/{idFactura}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsAsync<object>();
                    return Ok(data);
                }
                return StatusCode(response.StatusCode);
            }
        }

        // POST api/Facturacion/Procesar
        [HttpPost]
        [Route("Procesar")]
        public async Task<IHttpActionResult> Procesar([FromBody] object pedido)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync(urlCore + "facturacion/procesar", pedido);
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