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
    public partial class DetallePedido : System.Web.UI.Page
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
                CargarDetalle();
            }
        }

        private void CargarDetalle()
        {
            string parametro = Request.QueryString["id"];
            int id;

            if (!int.TryParse(parametro, out id))
            {
                MostrarError();
                return;
            }

            // Obtener detalle de la factura desde Core
            List<DetalleItemDTO> detalles = ObtenerDetalleDesdeAPI(id);

            if (detalles == null || detalles.Count == 0)
            {
                MostrarError();
                return;
            }

            // Llenar cabecera
            lblId.Text = "#" + id;
            lblIdDetalle.Text = "#" + id;

            // Calcular subtotal sin ITBIS
            decimal subtotal = 0;
            foreach (var item in detalles)
                subtotal += item.SubTotal;

            // Calcular ITBIS 18%
            decimal itbis = Math.Round(subtotal * 0.18m, 2);
            decimal totalConItbis = Math.Round(subtotal + itbis, 2);

            lblTotal.Text = totalConItbis.ToString("C2");
            lblTotalGeneral.Text = totalConItbis.ToString("C2");
            lblSubtotalSinItbis.Text = subtotal.ToString("C2");
            lblItbis.Text = itbis.ToString("C2");

            // Estado fijo — viene de la factura
            lblEstado.Text = "Pagada";
            lblEstado.CssClass = "badge-estado estado-pagada";

            // Llenar GridView con los productos
            gvProductos.DataSource = detalles;
            gvProductos.DataBind();

            pnlDetalle.Visible = true;
        }

        private List<DetalleItemDTO> ObtenerDetalleDesdeAPI(int idFactura)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(UrlIntegracion + "facturacion/detalle/" + idFactura).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var json = response.Content.ReadAsStringAsync().Result;
                        return JsonConvert.DeserializeObject<List<DetalleItemDTO>>(json);
                    }
                }
            }
            catch { }

            return null;
        }

        private void MostrarError()
        {
            pnlDetalle.Visible = false;
            pnlError.Visible = true;
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

        // DTO que coincide con lo que devuelve Core
        private class DetalleItemDTO
        {
            public string Marca { get; set; }
            public string Modelo { get; set; }
            public string Medida { get; set; }
            public int Cantidad { get; set; }
            public decimal PrecioUnitario { get; set; }
            public decimal SubTotal { get; set; }
        }
    }
}