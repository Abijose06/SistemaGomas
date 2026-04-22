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
            // Bloqueamos el botón para que el usuario no le dé 10 veces seguidas
            btnIngresar.Enabled = false;
            btnIngresar.Text = "Conectando...";

            // Preparamos los datos
            var peticionLogin = new
            {
                Correo = txtUsuario.Text,
                Password = txtPassword.Text
            };

            bool conexionExitosa = false;

            // --- INTENTO 1: MODO ONLINE (API/BASE DE DATOS) ---
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);
                    client.Timeout = TimeSpan.FromSeconds(4); // Si en 4 segundos no hay internet, saltamos al offline

                    var content = new System.Net.Http.StringContent(JsonConvert.SerializeObject(peticionLogin), System.Text.Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("api/usuarios/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonString = await response.Content.ReadAsStringAsync();
                        dynamic datosUsuario = JsonConvert.DeserializeObject(jsonString);

                        this.RolUsuario = datosUsuario.Rol;
                        MessageBox.Show("Conexión Establecida.\nBienvenido, " + datosUsuario.NombreCompleto, "Modo Online", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        conexionExitosa = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Documento o Contraseña incorrectos en la Base de Datos central.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        conexionExitosa = true; // La red funcionó, pero la clave estaba mal, así que no vamos al offline.
                    }
                }
            }
            catch (Exception)
            {
                // Hubo un error de red (Timeout, servidor apagado, sin internet).
                // conexionExitosa sigue siendo 'false', así que el código seguirá abajo.
            }

            // --- INTENTO 2: MODO OFFLINE (JSON LOCAL) ---
            // Solo entramos aquí si el intento 1 falló por falta de red
            if (!conexionExitosa)
            {
                try
                {
                    var usuarioLocal = GestorOffline.ValidarLoginJSON(txtUsuario.Text, txtPassword.Text);

                    if (usuarioLocal != null)
                    {
                        this.RolUsuario = usuarioLocal.Rol;
                        MessageBox.Show("Servidor no disponible. Iniciando en MODO OFFLINE de respaldo.\n\nBienvenido, " + usuarioLocal.Nombres, "Acceso Local", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Sin conexión al Servidor y las credenciales no coinciden con ninguna cuenta de emergencia local.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error crítico al leer archivo de emergencia: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            // Restauramos el botón si falló el login
            btnIngresar.Enabled = true;
            btnIngresar.Text = "Iniciar Sesión";
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
        }
    }
}