using System;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CajaGomasPOS
{
    public partial class FormLogin : Form
    {
        public FormLogin()
        {
            InitializeComponent();
        }

        // Variable pública para que la Caja sepa quién entró
        public string RolUsuario = "";

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            // 1. Tu cadena de conexión al archivo .mdf local
            string conexionBD = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\Users\\dewri\\Desktop\\SIstemaGomas\\Caja\\CajaGomasPOS\\App_Data\\GomasDB.mdf;Integrated Security=True";

            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                try
                {
                    conexion.Open();

                    // 2. Buscamos al usuario por Documento y Clave (Usamos @ para evitar hackeos)
                    // Según tu tabla: Documento es el ID visual y ClaveHash es la contraseña
                    string query = "SELECT Rol FROM tblUsuario WHERE Documento = @doc AND ClaveHash = @pass AND Estado = 1";

                    using (SqlCommand cmd = new SqlCommand(query, conexion))
                    {
                        cmd.Parameters.AddWithValue("@doc", txtUsuario.Text); // Aquí el usuario pone su Documento
                        cmd.Parameters.AddWithValue("@pass", txtPassword.Text);

                        object resultado = cmd.ExecuteScalar();

                        if (resultado != null)
                        {
                            // 3. Si lo encuentra, guardamos el Rol y cerramos con OK
                            RolUsuario = resultado.ToString();
                            this.DialogResult = DialogResult.OK;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Documento o Clave incorrectos.\nO el usuario está inactivo.", "Error de Acceso", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error de conexión: " + ex.Message);
                }
            }
        }

        private void FormLogin_Load(object sender, EventArgs e)
        {
            // Puedes dejar esto vacío si no necesitas cargar nada al inicio
        }

    }
}