using System;
using System.Windows.Forms;
using Core.Models; // Para acceder a la clase Producto y GomasContext

namespace Core.DataEntry
{
    public partial class frmProducto : Form
    {
        public frmProducto()
        {
            InitializeComponent();
        }

        private void btnInsertarProducto_Click(object sender, EventArgs e)
        {
            // 1. Validación básica (Evitar textos en blanco)
            if (string.IsNullOrWhiteSpace(txtMarca.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtMedida.Text))
            {
                MessageBox.Show("Por favor, complete todos los campos de texto (Marca, Modelo y Medida).", "Campos vacíos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Validación de lógica de negocio (Evitar precios negativos o en cero)
            if (nudPrecioVenta.Value <= 0 || nudCosto.Value <= 0)
            {
                MessageBox.Show("El precio de venta y el costo deben ser mayores a cero.", "Valores inválidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 3. Inserción con Entity Framework
                using (var db = new GomasContext())
                {
                    var nuevoProducto = new Producto
                    {
                        Marca = txtMarca.Text,
                        Modelo = txtModelo.Text,
                        Medida = txtMedida.Text,
                        PrecioVenta = nudPrecioVenta.Value,
                        Costo = nudCosto.Value,
                        Estado = true // Activo por defecto
                    };

                    db.Productos.Add(nuevoProducto);
                    db.SaveChanges();

                    MessageBox.Show("El producto ha sido registrado exitosamente en el catálogo.", "Operación Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LimpiarCampos();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hubo un error al guardar el producto en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Método auxiliar para limpiar las cajas después de guardar
        private void LimpiarCampos()
        {
            txtMarca.Clear();
            txtModelo.Clear();
            txtMedida.Clear();

            // Regresamos los contadores numéricos a su valor mínimo (usualmente 0)
            nudPrecioVenta.Value = nudPrecioVenta.Minimum;
            nudCosto.Value = nudCosto.Minimum;

            txtMarca.Focus(); // Colocamos el cursor de nuevo en la primera caja
        }
    }
}