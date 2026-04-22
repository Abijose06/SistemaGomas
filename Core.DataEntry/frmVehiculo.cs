using System;
using System.Linq;
using System.Windows.Forms;
using Core.Models;

namespace Core.DataEntry
{
    public partial class frmVehiculo : Form
    {
        public frmVehiculo()
        {
            InitializeComponent();
        }

        private void frmVehiculo_Load(object sender, EventArgs e)
        {
            // Cargamos los clientes para saber a quién le asignamos el vehículo
            try
            {
                using (var db = new GomasContext())
                {
                    // Consultamos los usuarios que son clientes para llenar el combo
                    var clientes = db.Usuarios.Where(u => u.Rol == "Cliente").ToList();
                    cmbCliente.DataSource = clientes;
                    cmbCliente.DisplayMember = "Nombres"; // Lo que ves
                    cmbCliente.ValueMember = "IdUsuario"; // El ID que guardamos
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message);
            }
        }

        private void btnInsertarVehiculo_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlaca.Text) || cmbCliente.SelectedValue == null)
            {
                MessageBox.Show("La placa y el cliente son obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = new GomasContext())
                {
                    var nuevoVehiculo = new Vehiculo
                    {
                        IdCliente = Convert.ToInt32(cmbCliente.SelectedValue),
                        Marca = txtMarca.Text,
                        Modelo = txtModelo.Text,
                        Año = Convert.ToInt32(nudAño.Value),
                        Placa = txtPlaca.Text,
                        Chasis = txtChassis.Text,
                        Estado = true
                    };

                    db.Vehiculos.Add(nuevoVehiculo);
                    db.SaveChanges();

                    MessageBox.Show("Vehículo registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al registrar vehículo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LimpiarCampos()
        {
            txtMarca.Clear();
            txtModelo.Clear();
            txtPlaca.Clear();
            txtChassis.Clear();
            nudAño.Value = DateTime.Now.Year;
            txtMarca.Focus();
        }
    }
}