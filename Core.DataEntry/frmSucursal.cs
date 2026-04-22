using System;
using System.Windows.Forms;
using Core.Models; // Para acceder a Sucursal y GomasContext

namespace Core.DataEntry
{
    public partial class frmSucursal : Form
    {
        public frmSucursal()
        {
            InitializeComponent();
        }

        private void btnInsertarSucursal_Click(object sender, EventArgs e)
        {
            // 1. Validación básica
            if (string.IsNullOrWhiteSpace(txtDireccion.Text) || string.IsNullOrWhiteSpace(txtTelefono.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos.", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Usamos el contexto de Entity Framework para guardar
                using (var db = new GomasContext())
                {
                    var nuevaSucursal = new Sucursal
                    {
                        Direccion = txtDireccion.Text,
                        Telefono = txtTelefono.Text,
                        Estado = true // La creamos activa por defecto
                    };

                    db.Sucursales.Add(nuevaSucursal);
                    db.SaveChanges();

                    MessageBox.Show("Sucursal registrada exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error al guardar la sucursal: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método para limpiar las cajas después de guardar
        private void LimpiarCampos()
        {
            txtDireccion.Clear();
            txtTelefono.Clear();
            txtDireccion.Focus(); // Pone el cursor de nuevo en dirección
        }
    }
}