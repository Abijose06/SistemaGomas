using System;
using System.Windows.Forms;
using Core.Models;

namespace Core.DataEntry
{
    public partial class frmServicio : Form
    {
        public frmServicio()
        {
            InitializeComponent();
        }

        private void btnInsertarServicio_Click(object sender, EventArgs e)
        {
            // 1. Validación de campos obligatorios
            if (string.IsNullOrWhiteSpace(txtNombreServicio.Text))
            {
                MessageBox.Show("El nombre del servicio es obligatorio.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nudPrecio.Value <= 0)
            {
                MessageBox.Show("El precio debe ser mayor a cero.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = new GomasContext())
                {
                    // 2. Mapeo de datos al objeto Servicio
                    var nuevoServicio = new Servicio
                    {
                        NombreServicio = txtNombreServicio.Text,
                        Precio = nudPrecio.Value,
                        Descripcion = txtDescripcion.Text,
                        Estado = true
                    };

                    // 3. Guardado en base de datos
                    db.Servicios.Add(nuevoServicio);
                    db.SaveChanges();

                    MessageBox.Show("Servicio registrado correctamente en el sistema.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar guardar el servicio: " + ex.Message, "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtNombreServicio.Clear();
            txtDescripcion.Clear();
            nudPrecio.Value = nudPrecio.Minimum;
            txtNombreServicio.Focus();
        }
    }
}