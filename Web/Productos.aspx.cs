using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web.UI;
using WebGomas.Models;

namespace WebGomas
{
    public partial class Productos : System.Web.UI.Page
    {
        string UrlIntegracion = ConfigurationManager.AppSettings["UrlIntegracion"];

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["usuario"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            if (!IsPostBack)
            {
                ActualizarHeader();
                CargarProductos();
            }
        }

        private void CargarProductos()
        {
            List<Producto> productos = ObtenerProductosDesdeAPI();

            lblCantidad.Text = productos.Count + " productos";
            rptProductos.DataSource = productos;
            rptProductos.DataBind();
        }

        private List<Producto> ObtenerProductosDesdeAPI()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(UrlIntegracion + "productos/catalogo/1").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        var dtos = JsonConvert.DeserializeObject<List<ProductoDTO>>(json);

                        var lista = new List<Producto>();
                        foreach (var dto in dtos)
                        {
                            lista.Add(new Producto
                            {
                                Id = dto.IdProducto,
                                Nombre = dto.Modelo,
                                Marca = dto.Marca,
                                Precio = dto.PrecioVenta,
                                ImagenUrl = ObtenerImagenPorModelo(dto.Modelo)
                            });
                        }
                        return lista;
                    }
                }
            }
            catch { }

            return ObtenerDatosSimulados();
        }

        // -------------------------------------------------------
        // Asigna la imagen local según el nombre del modelo
        // -------------------------------------------------------
        private string ObtenerImagenPorModelo(string modelo)
        {
            if (modelo == null) return "images/GomaPilotSport4.png";

            if (modelo.Contains("Pilot Sport")) return "images/GomaPilotSport4.png";
            if (modelo.Contains("Eagle")) return "images/GomaEagleF1Asymmetric.png";
            if (modelo.Contains("Cinturato")) return "images/GomaCinturatoP7.png";
            if (modelo.Contains("Conti")) return "images/GomaContiSportContact.png";
            if (modelo.Contains("Potenza")) return "images/GomaPotenzaS007.png";
            if (modelo.Contains("Ventus")) return "images/GomaVentusS1Evo3.png";
            if (modelo.Contains("Proxes")) return "images/GomaProxesSport.png";
            if (modelo.Contains("Primacy")) return "images/GomaPrimacy4.png";

            return "images/GomaPilotSport4.png";
        }

        // -------------------------------------------------------
        // Respaldo si Core no está disponible
        // -------------------------------------------------------
        private List<Producto> ObtenerDatosSimulados()
        {
            return new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Pilot Sport 4",       Marca = "Michelin",    Precio = 185.00m, ImagenUrl = "images/GomaPilotSport4.png"       },
                new Producto { Id = 2, Nombre = "Eagle F1 Asymmetric", Marca = "Goodyear",    Precio = 160.00m, ImagenUrl = "images/GomaEagleF1Asymmetric.png" },
                new Producto { Id = 3, Nombre = "Cinturato P7",        Marca = "Pirelli",     Precio = 145.00m, ImagenUrl = "images/GomaCinturatoP7.png"       },
                new Producto { Id = 4, Nombre = "ContiSportContact",   Marca = "Continental", Precio = 170.00m, ImagenUrl = "images/GomaContiSportContact.png" },
                new Producto { Id = 5, Nombre = "Potenza S007",        Marca = "Bridgestone", Precio = 155.00m, ImagenUrl = "images/GomaPotenzaS007.png"       },
                new Producto { Id = 6, Nombre = "Ventus S1 Evo3",      Marca = "Hankook",     Precio = 138.00m, ImagenUrl = "images/GomaVentusS1Evo3.png"      },
                new Producto { Id = 7, Nombre = "Proxes Sport",        Marca = "Toyo",        Precio = 142.00m, ImagenUrl = "images/GomaProxesSport.png"       },
                new Producto { Id = 8, Nombre = "Primacy 4+",          Marca = "Michelin",    Precio = 167.00m, ImagenUrl = "images/GomaPrimacy4.png"          }
            };
        }

        private void ActualizarHeader()
        {
            if (Session["usuario"] != null)
            {
                phLogueado.Visible = true;
                phNoLogueado.Visible = false;
                string nombre = Session["nombre"].ToString();
                lblUsuario.Text = nombre;
                lblAvatar.Text = nombre.Substring(0, 1).ToUpper();
            }
            else
            {
                phLogueado.Visible = false;
                phNoLogueado.Visible = true;
            }
        }

        private class ProductoDTO
        {
            public int IdProducto { get; set; }
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Medida { get; set; }
            public decimal PrecioVenta { get; set; }
            public int StockActual { get; set; }
        }
    }
}