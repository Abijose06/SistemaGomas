using System;
using System.Windows.Forms;

namespace Core.DataEntry
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }

        // 1. Botón del menú para Empleados
        private void empleadoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEmpleado frm = new frmEmpleado();
            frm.MdiParent = this; 
            frm.Show(); 
        }

        // 2. Botón del menú para Clientes
        private void clienteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCliente frm = new frmCliente();
            frm.MdiParent = this;
            frm.Show();
        }

        // 3. Botón del menú para Sucursales
        private void sucursalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSucursal frm = new frmSucursal();
            frm.MdiParent = this;
            frm.Show();
        }

        // 4. Botón del menú para Productos
        private void productoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Verifica si ya hay un frmProducto abierto en la colección de ventanas "hijas"
            foreach (Form formHijo in this.MdiChildren)
            {
                if (formHijo.GetType() == typeof(frmProducto))
                {
                    formHijo.Focus();
                    return;
                }
            }

            // Si no lo encontró abierto, lo crea normalmente
            frmProducto frm = new frmProducto();
            frm.MdiParent = this;
            frm.Show();
        }

        // 5. Botón del menú para Servicios
        private void servicioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmServicio frm = new frmServicio();
            frm.MdiParent = this;
            frm.Show();
        }

        // 6. Botón del menú para Vehículos
        private void vehiculoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmVehiculo frm = new frmVehiculo();
            frm.MdiParent = this;
            frm.Show();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); // Cierra el programa por completo
        }
    }
}