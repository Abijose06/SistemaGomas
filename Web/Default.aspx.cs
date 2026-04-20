using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.UI;
using Newtonsoft.Json;

namespace WebGomas
{
    public partial class _Default : Page
    {
        
        private string UrlIntegracion = ConfigurationManager.AppSettings["UrlIntegracion"];


        protected async void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                await CargarCatalogoWeb();
            }
        }

        private async Task CargarCatalogoWeb()
        {
            try
            {
                // Ignorar advertencias de certificados locales
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);

                    // Conexión silenciosa al Core para acceder a la base de datos
                    var response = await client.GetAsync("api/productos/catalogo/1");

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();

                        // Los datos se descargan y se quedan listos en memoria
                        // sin alterar el diseño HTML original
                        var productos = JsonConvert.DeserializeObject<List<ProductoWebDTO>>(jsonString);
                    }
                }
            }
            catch (Exception ex)
            {
                // Error capturado en silencio para no romper la página
            }
        }

        public class ProductoWebDTO
        {
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Medida { get; set; }
            public decimal PrecioVenta { get; set; }
            public int StockActual { get; set; }
        }
    }
}