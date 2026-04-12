using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CajaGomasPOS
{
    public partial class FormCobro : Form
    {
        public FormCobro()
        {
            InitializeComponent();
        }

        // 1. Variables para compartir con el recibo
        public decimal TotalPagar = 0;
        public string MetodoPago = "";
        public decimal EfectivoEntregado = 0;
        public decimal CambioDevuelto = 0;
        public decimal EfectivoEnCaja = 0;

        private void FormCobro_Load(object sender, EventArgs e)
        {
          

            lblTotalPagar.Text = TotalPagar.ToString("C");
            cmbMetodoPago.SelectedIndex = 0; // Selecciona "Efectivo" por defecto
        }

        private void btnCobrar_Click_1(object sender, EventArgs e)
        {
            decimal efectivo = 0;
            try { efectivo = Convert.ToDecimal(txtEfectivo.Text); } catch { }

            if (efectivo >= TotalPagar)
            {
                // 1. Calculamos la devuelta real
                decimal devuelta = efectivo - TotalPagar;

                // --- VALIDACIÓN DE SEGURIDAD ---
                if (cmbMetodoPago.Text == "Efectivo" && devuelta > EfectivoEnCaja)
                {
                    MessageBox.Show($"No hay suficiente efectivo en la gaveta para dar esta devuelta.\n\nDevuelta requerida: {devuelta.ToString("C")}\nDisponible en caja: {EfectivoEnCaja.ToString("C")}", "Fondo Insuficiente", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 2. Si todo está bien, guardamos los datos para la factura
                MetodoPago = cmbMetodoPago.Text;
                EfectivoEntregado = efectivo;
                CambioDevuelto = devuelta;

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("El efectivo no es suficiente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void cmbMetodoPago_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbMetodoPago.Text == "Tarjeta")
            {
                lblTextoEfectivo.Text = "Monto a Cobrar:";
                txtEfectivo.Text = TotalPagar.ToString("0.00");
                txtEfectivo.Enabled = false;
                lblDevuelta.Text = "$0.00";
            }
            else
            {
                lblTextoEfectivo.Text = "Efectivo Recibido:";
                txtEfectivo.Text = "";
                txtEfectivo.Enabled = true;
                lblDevuelta.Text = "$0.00";
            }
        }

        // 3. Evento: Mientras el cajero escribe el monto en efectivo
        private void txtEfectivo_TextChanged_1(object sender, EventArgs e)
        {
            if (cmbMetodoPago.Text == "Tarjeta") return; // Si es tarjeta no calculamos nada

            try
            {
                decimal efectivo = Convert.ToDecimal(txtEfectivo.Text);
                decimal devuelta = efectivo - TotalPagar;
                if (devuelta < 0) devuelta = 0;
                lblDevuelta.Text = devuelta.ToString("C");
            }
            catch { lblDevuelta.Text = "$0.00"; }
        }
    }
}
