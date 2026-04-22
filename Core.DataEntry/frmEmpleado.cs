using System;
using System.Linq;
using System.Windows.Forms;
using Core.Models;
using Core.Helpers;

namespace Core.DataEntry
{
    public partial class frmEmpleado : Form
    {
        public frmEmpleado()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        // 1. Esto se ejecuta automáticamente cuando se abre la ventana
        private void frmEmpleado_Load(object sender, EventArgs e)
        {
            // Llenar el ComboBox de Roles manualmente
            cmbRol.Items.Add("Empleado");
            cmbRol.Items.Add("Admin");
            cmbRol.SelectedIndex = 0; // Selecciona "Empleado" por defecto

            // Llenar el ComboBox de Sucursales consultando la BD
            try
            {
                using (var db = new GomasContext())
                {
                    // Asumimos que tienes un DbSet llamado Sucursales en tu GomasContext
                    var sucursales = db.Sucursales.ToList();
                    cmbSucursal.DataSource = sucursales;
                    cmbSucursal.DisplayMember = "Direccion";
                    cmbSucursal.ValueMember = "IdSucursal";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Aviso: No se pudieron cargar las sucursales. " + ex.Message);
            }
        }

        // 2. El evento de tu botón Insertar
        private void btnInsertarEmpleado_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtDocumento.Text) ||
                string.IsNullOrWhiteSpace(txtContraseña.Text) ||
                string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                MessageBox.Show("Por favor llene los campos obligatorios.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = new GomasContext())
                {
                    using (var transaccion = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // A. Creamos el Usuario
                            var nuevoUsuario = new Usuario
                            {
                                TipoDocumento = Convert.ToInt32(txtTipoDocumento.Text),
                                Documento = txtDocumento.Text,
                                ClaveHash = SeguridadHelper.CalcularHash(txtContraseña.Text),
                                Rol = cmbRol.SelectedItem.ToString(), // Lee lo que seleccionaste
                                Nombres = txtNombres.Text,
                                Apellidos = txtApellidos.Text,
                                Telefono = txtTelefono.Text,
                                Correo = txtCorreo.Text,
                                Estado = true
                            };

                            db.Usuarios.Add(nuevoUsuario);
                            db.SaveChanges(); // Guarda y nos devuelve el IdUsuario

                            // B. Creamos el Empleado
                            var nuevoEmpleado = new Empleado
                            {
                                IdUsuario = nuevoUsuario.IdUsuario,
                                Sueldo = nudSueldo.Value, // Propiedad .Value para el NumericUpDown
                                FechaIngreso = dtpFechaIngreso.Value.Date, // Propiedad .Value para la fecha
                                IdSucursal = Convert.ToInt32(cmbSucursal.SelectedValue), // Saca el ID oculto
                                Estado = true
                            };

                            db.Empleados.Add(nuevoEmpleado);
                            db.SaveChanges();

                            transaccion.Commit();

                            MessageBox.Show("El empleado ha sido registrado exitosamente.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LimpiarCampos();
                        }
                        catch (Exception ex)
                        {
                            transaccion.Rollback();
                            MessageBox.Show("Error al guardar en la base de datos: " + ex.Message, "Error de BD", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error general. Verifique los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar para resetear todo a su estado original
        private void LimpiarCampos()
        {
            txtTipoDocumento.Clear();
            txtDocumento.Clear();
            txtContraseña.Clear();
            txtNombres.Clear();
            txtApellidos.Clear();
            txtTelefono.Clear();
            txtCorreo.Clear();

            nudSueldo.Value = nudSueldo.Minimum; // Lo devuelve a 0
            dtpFechaIngreso.Value = DateTime.Now; // Lo devuelve a hoy
            cmbRol.SelectedIndex = 0;
        }

    }
}