using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CajaGomasPOS
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        // Variable pública intacta para que la Caja sepa quién entró
        public string RolUsuario = "";
        private string UrlIntegracion = ConfigurationManager.AppSettings["UrlIntegracion"];


        private async void btnIngresar_Click(object sender, EventArgs e)
        {
            // 1. Armamos el paquete EXACTAMENTE como lo pide la clase LoginRequest de tu compañero
            var peticionLogin = new
            {
                TipoDocumento = 1, // Asumimos 1 por defecto (ej. Cédula)
                Documento = txtUsuario.Text,
                Password = txtPassword.Text
            };

            using (var client = new HttpClient())
            {
                try
                {
                    // IMPORTANTE: Cambia "tu-puerto" por el puerto de tu Core
                    client.BaseAddress = new Uri(UrlIntegracion);
                    var content = new StringContent(JsonConvert.SerializeObject(peticionLogin), Encoding.UTF8, "application/json");

                    // 2. Apuntamos a la ruta real que ya programó tu compañero
                    var response = await client.PostAsync("api/usuarios/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        dynamic datosUsuario = JsonConvert.DeserializeObject(jsonString);

                        // 3. Leemos el Rol y el NombreCompleto tal como los devuelve su API
                        RolUsuario = datosUsuario.Rol;
                        string nombreAtendido = datosUsuario.NombreCompleto;

                        MessageBox.Show("Bienvenido al sistema, " + nombreAtendido, "Acceso Concedido", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Documento o Clave incorrectos.\nO el usuario está inactivo en la base central.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión con el Servidor Core: " + ex.Message, "Fallo de Red", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
        }
    }
}