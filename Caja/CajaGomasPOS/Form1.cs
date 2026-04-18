using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;

namespace CajaGomasPOS
{
    public partial class Form1 : Form
    {
        // 🚀 AQUI CONFIGURAS LA RUTA AL CEREBRO (Sustituye por tu puerto real)
        private string UrlCore = "https://localhost:44376/";

        public Form1()
        {
            InitializeComponent();
        }

        string reciboMetodo = "";
        decimal reciboEfectivo = 0;
        decimal reciboDevuelta = 0;

        public decimal FondoCaja = 0;
        public decimal TotalEfectivoDelDia = 0;
        public bool ModoSupervisor = false;
        public MenuStrip MenuSuperior;
        public ToolStripMenuItem menuAdmin;
        public bool estaLogueado = false;

        public class ArticuloPOS
        {
            public int IdArticulo { get; set; }
            public string Tipo { get; set; }
            public string Marca { get; set; }
            public string Medida { get; set; }
            public string Modelo { get; set; }
            public decimal Precio { get; set; }
            public int Stock { get; set; }
        }

        List<ArticuloPOS> inventario = new List<ArticuloPOS>();
        // Pon esto justo debajo de public class ArticuloPOS { ... }
        public class UsuarioCombo
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
        }

        public void ActualizarGaveta()
        {
            decimal totalEnGaveta = FondoCaja + TotalEfectivoDelDia;
            lblDineroCaja.Text = "Dinero en Gaveta: " + totalEnGaveta.ToString("C");
        }

        private void GuardarVentaOffline(string jsonRespaldo)
        {
            try
            {
                System.IO.File.AppendAllText("Backup_VentasFallidas.json", jsonRespaldo + "\n");
                MessageBox.Show("Venta guardada en modo OFFLINE. Deberá sincronizarse luego.");
            }
            catch { }
        }

        private void CrearMenuSuperior()
        {
            MenuSuperior = new MenuStrip();
            MenuSuperior.BackColor = Color.LightGray;

            ToolStripMenuItem menuSistema = new ToolStripMenuItem("⚙️ Sistema");
            menuSistema.DropDownItems.Add(new ToolStripMenuItem("Cambiar Usuario / Iniciar Sesión", null, (s, e) => MostrarLogin()));
            menuSistema.DropDownItems.Add(new ToolStripMenuItem("Salir del Programa", null, (s, e) => this.Close()));

            ToolStripMenuItem menuCaja = new ToolStripMenuItem("💰 Operaciones de Caja");
            menuCaja.DropDownItems.Add(new ToolStripMenuItem("Apertura de Caja (Ingresar Fondo)", null, (s, e) => AbrirCaja()));
            menuCaja.DropDownItems.Add(new ToolStripMenuItem("Anular Venta Actual", null, btnAnularVenta_Click));
            menuCaja.DropDownItems.Add(new ToolStripSeparator());
            menuCaja.DropDownItems.Add(new ToolStripMenuItem("Cierre de Turno (Cuadre)", null, btnCierreCaja_Click));

            menuAdmin = new ToolStripMenuItem("🛡️ Administración");
            menuAdmin.DropDownItems.Add(new ToolStripMenuItem("Panel Supervisor (Inventario y Ventas)", null, btnSupervisor_Click));

            MenuSuperior.Items.Add(menuSistema);
            MenuSuperior.Items.Add(menuCaja);
            MenuSuperior.Items.Add(menuAdmin);
            this.MainMenuStrip = MenuSuperior;
            this.Controls.Add(MenuSuperior);
        }

        private void MostrarLogin()
        {
            FormLogin pantallaLogin = new FormLogin();
            pantallaLogin.StartPosition = FormStartPosition.CenterParent;
            if (pantallaLogin.ShowDialog() == DialogResult.OK)
            {
                estaLogueado = true;
                if (pantallaLogin.RolUsuario == "Admin" || pantallaLogin.RolUsuario == "Administrador")
                {
                    ModoSupervisor = true;
                    menuAdmin.Visible = true;
                    this.Text = "Caja POS - USUARIO: ADMINISTRADOR";
                }
                else
                {
                    ModoSupervisor = false;
                    menuAdmin.Visible = false;
                    this.Text = "Caja POS - USUARIO: CAJERO";
                }
            }
            else if (estaLogueado == false)
            {
                Environment.Exit(0);
            }
        }

        private void AbrirCaja()
        {
            Form formFondo = new Form() { Width = 300, Height = 150, Text = "Apertura de Caja", StartPosition = FormStartPosition.CenterScreen, FormBorderStyle = FormBorderStyle.FixedDialog };
            Label lblTexto = new Label() { Left = 20, Top = 20, Text = "Ingrese el fondo inicial ($):", AutoSize = true };
            TextBox txtFondo = new TextBox() { Left = 20, Top = 50, Width = 150 };
            Button btnAceptar = new Button() { Text = "Abrir Caja", Left = 180, Top = 48, Width = 80 };
            btnAceptar.Click += (sender, e) => {
                try
                {
                    FondoCaja = Convert.ToDecimal(txtFondo.Text);
                    ActualizarGaveta();
                    MessageBox.Show("Caja abierta exitosamente con " + FondoCaja.ToString("C"), "Apertura", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    formFondo.Close();
                }
                catch { MessageBox.Show("Ingrese un monto válido."); }
            };
            formFondo.Controls.Add(lblTexto); formFondo.Controls.Add(txtFondo); formFondo.Controls.Add(btnAceptar);
            formFondo.ShowDialog();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            docImprimir.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(docImprimir_PrintPage);
            this.FormClosing += (s, ev) => {
                if (TotalEfectivoDelDia > 0 && ev.CloseReason != CloseReason.ApplicationExitCall)
                {
                    MessageBox.Show("⚠️ ALERTA: Debe realizar su cuadre obligatorio en 'Caja -> Cierre de Turno'.", "Cierre Bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    ev.Cancel = true;
                }
            };

            CrearMenuSuperior();
            MostrarLogin();

            // CARGA DE DATOS POR API
            await CargarPersonasDesdeAPI();
            await CargarInventarioDesdeAPI();

            ActualizarGaveta();
            cmbSucursal.Items.Add("Principal - Santo Domingo");
            cmbSucursal.SelectedIndex = 0;

            cmbTipoItem.Items.Add("Producto (Goma)");
            cmbTipoItem.Items.Add("Servicio (Taller)");
            cmbTipoItem.SelectedIndex = 0;
        }

        private async Task CargarPersonasDesdeAPI()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlCore);

                    // Consumimos Clientes
                    var resCli = await client.GetAsync("api/usuarios/clientes");
                    if (resCli.IsSuccessStatusCode)
                    {
                        var json = await resCli.Content.ReadAsStringAsync();
                        // 🛠️ ARREGLO: Deserializamos usando la clase estricta en lugar de dynamic
                        var listaClientes = JsonConvert.DeserializeObject<List<UsuarioCombo>>(json);
                        cmbCliente.DataSource = listaClientes;
                        cmbCliente.DisplayMember = "Nombre";
                        cmbCliente.ValueMember = "Id";
                    }

                    // Consumimos Empleados
                    var resEmp = await client.GetAsync("api/usuarios/empleados");
                    if (resEmp.IsSuccessStatusCode)
                    {
                        var json = await resEmp.Content.ReadAsStringAsync();
                        // 🛠️ ARREGLO: Deserializamos usando la clase estricta
                        var listaEmpleados = JsonConvert.DeserializeObject<List<UsuarioCombo>>(json);
                        cmbEmpleado.DataSource = listaEmpleados;
                        cmbEmpleado.DisplayMember = "Nombre";
                        cmbEmpleado.ValueMember = "Id";
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error de red al cargar personal: " + ex.Message); }
        }

        private async Task CargarInventarioDesdeAPI()
        {
            inventario.Clear();
            int idSucursalActual = 1;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlCore);

                    // 1. Traer los neumáticos del catálogo
                    var resGomas = await client.GetAsync($"api/productos/catalogo/{idSucursalActual}");
                    if (resGomas.IsSuccessStatusCode)
                    {
                        var jsonGomas = await resGomas.Content.ReadAsStringAsync();
                        var gomas = JsonConvert.DeserializeObject<List<dynamic>>(jsonGomas);
                        foreach (var g in gomas)
                        {
                            inventario.Add(new ArticuloPOS
                            {
                                IdArticulo = (int)g.IdProducto,
                                Tipo = "Goma",
                                Marca = (string)g.Marca,
                                Medida = (string)g.Medida,
                                Modelo = (string)g.Modelo,
                                Precio = (decimal)g.PrecioVenta,
                                Stock = (int)g.StockActual
                            });
                        }
                    }

                    // 2. Traer los servicios
                    var resServ = await client.GetAsync("api/servicios/catalogo");
                    if (resServ.IsSuccessStatusCode)
                    {
                        var jsonServ = await resServ.Content.ReadAsStringAsync();
                        var servicios = JsonConvert.DeserializeObject<List<dynamic>>(jsonServ);
                        foreach (var s in servicios)
                        {
                            inventario.Add(new ArticuloPOS
                            {
                                IdArticulo = (int)s.IdServicio,
                                Tipo = "Servicio",
                                Marca = "",
                                Medida = "",
                                Modelo = (string)s.NombreServicio,
                                Precio = (decimal)s.Precio,
                                Stock = 999
                            });
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error de red al cargar inventario: " + ex.Message); }
        }

        private void cmbTipoItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbMarca.Items.Clear(); cmbMedida.Items.Clear(); cmbModelo.Items.Clear();
            if (cmbTipoItem.Text == "Producto (Goma)")
            {
                cmbMarca.Enabled = true; cmbMedida.Enabled = true; lblModelo.Text = "Modelo:";
                var marcas = inventario.Where(x => x.Tipo == "Goma").Select(x => x.Marca).Distinct().ToList();
                foreach (var m in marcas) cmbMarca.Items.Add(m);
            }
            else
            {
                cmbMarca.Enabled = false; cmbMedida.Enabled = false; lblModelo.Text = "Servicio:";
                var servicios = inventario.Where(x => x.Tipo == "Servicio").Select(x => x.Modelo).ToList();
                foreach (var s in servicios) cmbModelo.Items.Add(s);
            }
        }

        private void cmbMarca_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbMedida.Items.Clear(); cmbModelo.Items.Clear();
            if (cmbMarca.SelectedItem == null) return;
            var medidas = inventario.Where(x => x.Tipo == "Goma" && x.Marca == cmbMarca.Text).Select(x => x.Medida).Distinct().ToList();
            foreach (var m in medidas) cmbMedida.Items.Add(m);
        }

        private void cmbMedida_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbModelo.Items.Clear();
            if (cmbMedida.SelectedItem == null) return;
            var modelos = inventario.Where(x => x.Tipo == "Goma" && x.Marca == cmbMarca.Text && x.Medida == cmbMedida.Text).Select(x => x.Modelo).Distinct().ToList();
            foreach (var m in modelos) cmbModelo.Items.Add(m);
        }

        private void cmbModelo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbModelo.SelectedItem == null) return;
            ArticuloPOS item = cmbTipoItem.Text == "Producto (Goma)" ?
                inventario.FirstOrDefault(x => x.Marca == cmbMarca.Text && x.Medida == cmbMedida.Text && x.Modelo == cmbModelo.Text) :
                inventario.FirstOrDefault(x => x.Tipo == "Servicio" && x.Modelo == cmbModelo.Text);

            if (item != null)
            {
                lblPrecioVista.Text = "Precio: " + item.Precio.ToString("C");
                if (item.Tipo == "Goma")
                {
                    lblStockVista.Text = "Stock: " + item.Stock;
                    lblStockVista.ForeColor = item.Stock > 0 ? Color.DarkGreen : Color.Crimson;
                    lblStockVista.Visible = true;
                }
                else lblStockVista.Visible = false;
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.ColumnCount <= 5)
            {
                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.Name = "colIdOriginal";
                colId.Visible = false;
                dgvCarrito.Columns.Add(colId);
            }

            if (cmbModelo.SelectedItem == null)
            {
                MessageBox.Show("⚠️ Seleccione un Producto o Servicio.", "Falta selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int cant = (int)nudCantidad.Value;
            ArticuloPOS item = cmbTipoItem.Text.Contains("Goma") ?
                inventario.FirstOrDefault(x => x.Marca == cmbMarca.Text && x.Medida == cmbMedida.Text && x.Modelo == cmbModelo.Text) :
                inventario.FirstOrDefault(x => x.Tipo == "Servicio" && x.Modelo == cmbModelo.Text);

            if (item != null)
            {
                if (item.Tipo == "Goma" && (item.Stock <= 0 || cant > item.Stock))
                {
                    MessageBox.Show("❌ Stock insuficiente en inventario local.", "Sin Inventario", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (item.Tipo == "Goma") item.Stock -= cant;
                lblStockVista.Text = "Stock: " + item.Stock;
                string desc = item.Tipo == "Goma" ? $"{item.Marca} - {item.Medida} - {item.Modelo}" : item.Modelo;

                dgvCarrito.Rows.Add(cmbTipoItem.Text, desc, cant, item.Precio, cant * item.Precio, item.IdArticulo);
                RecalcularTotales();
            }
        }

        private void RecalcularTotales()
        {
            decimal sub = 0;
            foreach (DataGridViewRow f in dgvCarrito.Rows) sub += Convert.ToDecimal(f.Cells[4].Value);
            lblSubTotal.Text = sub.ToString("C");
            lblImpuesto.Text = (sub * 0.18m).ToString("C");
            lblTotal.Text = (sub * 1.18m).ToString("C");
        }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.CurrentRow != null)
            {
                string tipo = dgvCarrito.CurrentRow.Cells[0].Value.ToString();
                string desc = dgvCarrito.CurrentRow.Cells[1].Value.ToString();
                int cant = Convert.ToInt32(dgvCarrito.CurrentRow.Cells[2].Value);
                if (tipo == "Producto (Goma)")
                {
                    var item = inventario.FirstOrDefault(x => (x.Marca + " - " + x.Medida + " - " + x.Modelo) == desc);
                    if (item != null) item.Stock += cant;
                }
                dgvCarrito.Rows.Remove(dgvCarrito.CurrentRow);
                RecalcularTotales();
            }
        }

        private void btnAnularVenta_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count > 0 && MessageBox.Show("¿Anular?", "Anular", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (DataGridViewRow f in dgvCarrito.Rows)
                {
                    if (f.Cells[0].Value.ToString() == "Producto (Goma)")
                    {
                        string desc = f.Cells[1].Value.ToString();
                        int cant = Convert.ToInt32(f.Cells[2].Value);
                        var item = inventario.FirstOrDefault(x => (x.Marca + " - " + x.Medida + " - " + x.Modelo) == desc);
                        if (item != null) item.Stock += cant;
                    }
                }
                dgvCarrito.Rows.Clear(); RecalcularTotales();
            }
        }

        private async void btnFacturar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("🛒 El carrito está vacío.", "Carrito Vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            decimal subTotal = 0;
            foreach (DataGridViewRow f in dgvCarrito.Rows) subTotal += Convert.ToDecimal(f.Cells[4].Value);

            FormCobro cobro = new FormCobro();
            cobro.TotalPagar = subTotal * 1.18m;
            cobro.EfectivoEnCaja = FondoCaja + TotalEfectivoDelDia;

            if (cobro.ShowDialog() == DialogResult.OK)
            {
                reciboMetodo = cobro.MetodoPago;
                reciboEfectivo = cobro.EfectivoEntregado;
                reciboDevuelta = cobro.CambioDevuelto;

                var peticionVenta = new
                {
                    IdCliente = Convert.ToInt32(cmbCliente.SelectedValue),
                    IdEmpleado = Convert.ToInt32(cmbEmpleado.SelectedValue),
                    IdSucursal = 1,
                    IdVehiculo = 1,
                    MetodoPago = reciboMetodo,
                    Detalles = new List<object>()
                };

                foreach (DataGridViewRow f in dgvCarrito.Rows)
                {
                    char tipoItem = f.Cells[0].Value.ToString().Contains("Goma") ? 'P' : 'S';
                    int idArticulo = Convert.ToInt32(f.Cells[5].Value);

                    peticionVenta.Detalles.Add(new
                    {
                        TipoItem = tipoItem.ToString(),
                        IdProducto = tipoItem == 'P' ? idArticulo : (int?)null,
                        IdServicio = tipoItem == 'S' ? idArticulo : (int?)null,
                        Cantidad = Convert.ToInt32(f.Cells[2].Value),
                        PrecioUnitario = Convert.ToDecimal(f.Cells[3].Value)
                    });
                }

                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(UrlCore);
                        var content = new StringContent(JsonConvert.SerializeObject(peticionVenta), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("api/facturacion/procesar", content);

                        if (response.IsSuccessStatusCode)
                        {
                            if (reciboMetodo == "Efectivo") { TotalEfectivoDelDia += (reciboEfectivo - reciboDevuelta); ActualizarGaveta(); }
                            MessageBox.Show("✅ ¡Venta procesada y guardada exitosamente en la base central!");

                            previewImprimir.ShowDialog();
                            dgvCarrito.Rows.Clear(); RecalcularTotales();

                            await CargarInventarioDesdeAPI();
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("❌ El Servidor rechazó la venta:\n" + error, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fallo de red: No se pudo conectar con el Core central.", "Error Crítico", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    GuardarVentaOffline(JsonConvert.SerializeObject(peticionVenta));
                }
            }
        }

        private async void btnSupervisor_Click(object sender, EventArgs e)
        {
            Form formReporte = new Form { Text = "PANEL DE CONTROL - MODO SUPERVISOR", Size = new Size(850, 500), StartPosition = FormStartPosition.CenterScreen };
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            TabPage tabInv = new TabPage("📦 Inventario Local (Cacheado)");
            DataGridView dgvInv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvInv.DataSource = inventario.ToList();
            if (dgvInv.Columns["Precio"] != null) dgvInv.Columns["Precio"].DefaultCellStyle.Format = "N2";
            tabInv.Controls.Add(dgvInv);

            TabPage tabFact = new TabPage("💰 Ventas del Día (En Vivo)");
            DataGridView dgvFact = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            decimal sumaTodo = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlCore);
                    var response = await client.GetAsync("api/facturacion/resumen-diario/1");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        if (json.TrimStart().StartsWith("["))
                        {
                            var ventas = JsonConvert.DeserializeObject<List<dynamic>>(json);
                            dgvFact.DataSource = ventas;

                            foreach (DataGridViewColumn col in dgvFact.Columns)
                            {
                                if (col.Name == "TotalGeneral") col.DefaultCellStyle.Format = "N2";
                            }
                            foreach (var v in ventas) sumaTodo += Convert.ToDecimal(v.TotalGeneral);
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Error de red: " + ex.Message); }

            tabFact.Text = $"💰 Ventas (Total: {sumaTodo:C2})";
            tabFact.Controls.Add(dgvFact);

            tabs.TabPages.Add(tabInv);
            tabs.TabPages.Add(tabFact);
            formReporte.Controls.Add(tabs);
            formReporte.ShowDialog();
        }

        private void btnCierreCaja_Click(object sender, EventArgs e)
        {
            if (TotalEfectivoDelDia == 0)
            {
                if (MessageBox.Show("No se registraron ventas hoy. ¿Desea salir del sistema?", "Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                return;
            }

            Form formCuadre = new Form() { Width = 380, Height = 350, Text = "Cuadre de Caja Obligatorio", StartPosition = FormStartPosition.CenterScreen, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };

            Label lblInstruccion = new Label() { Left = 20, Top = 20, Text = "⚠️ SE REGISTRARON VENTAS EN EFECTIVO.\nIngrese el monto físico contado en gaveta:", AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = Color.DarkRed };
            TextBox txtContado = new TextBox() { Left = 20, Top = 60, Width = 150, Font = new Font("Arial", 14) };
            Button btnVerificar = new Button() { Text = "Verificar Dinero", Left = 180, Top = 58, Width = 150, Height = 30, Cursor = Cursors.Hand };

            Label lblResultado = new Label() { Left = 20, Top = 110, AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold), Text = "Esperando verificación..." };
            Button btnCerrarTurno = new Button() { Text = "Confirmar y Salir", Left = 110, Top = 240, Width = 150, Height = 40, Enabled = false, BackColor = Color.LightGray };

            btnVerificar.Click += (s, ev) =>
            {
                try
                {
                    decimal efectivoContado = Convert.ToDecimal(txtContado.Text);
                    decimal totalEsperado = FondoCaja + TotalEfectivoDelDia;
                    decimal diferencia = efectivoContado - totalEsperado;

                    if (diferencia == 0)
                    {
                        lblResultado.ForeColor = Color.DarkGreen;
                        lblResultado.Text = $"Efectivo Esperado: {totalEsperado:C}\nFísico Contado: {efectivoContado:C}\n\n✅ CUADRE PERFECTO.\nYa puede cerrar el sistema.";
                        btnCerrarTurno.Enabled = true;
                        btnCerrarTurno.BackColor = Color.LimeGreen;
                        txtContado.ReadOnly = true;
                        btnVerificar.Enabled = false;
                    }
                    else
                    {
                        string mensajeDiferencia = diferencia > 0 ? $"SOBRANTE: {diferencia:C}" : $"FALTANTE: {Math.Abs(diferencia):C}";
                        lblResultado.ForeColor = Color.Crimson;
                        lblResultado.Text = $"Efectivo Esperado: {totalEsperado:C}\nFísico Contado: {efectivoContado:C}\n\n❌ {mensajeDiferencia}\n\nERROR: El dinero no coincide.\nNo se permite el cierre hasta cuadrar.";
                        btnCerrarTurno.Enabled = false;
                        btnCerrarTurno.BackColor = Color.LightGray;
                        txtContado.Focus(); txtContado.SelectAll();
                    }
                }
                catch { MessageBox.Show("Por favor, ingrese un monto numérico válido.", "Error de formato"); }
            };

            btnCerrarTurno.Click += (s, ev) =>
            {
                MessageBox.Show("Turno cerrado correctamente. Vuelva pronto.", "Cierre Exitoso");
                Application.Exit();
            };

            formCuadre.Controls.Add(lblInstruccion); formCuadre.Controls.Add(txtContado); formCuadre.Controls.Add(btnVerificar); formCuadre.Controls.Add(lblResultado); formCuadre.Controls.Add(btnCerrarTurno);
            formCuadre.ShowDialog();
        }

        private void docImprimir_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics gfx = e.Graphics;
            Font fontTitulo = new Font("Courier New", 14, FontStyle.Bold);
            Font fontNormal = new Font("Courier New", 10);
            Font fontBold = new Font("Courier New", 10, FontStyle.Bold);

            StringFormat formatoCentro = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat formatoDerecha = new StringFormat { Alignment = StringAlignment.Far };

            int y = 20;
            int ancho = 300;
            string separador = "--------------------------------------";

            gfx.DrawString("PRECISION TIRE POS", fontTitulo, Brushes.Black, new RectangleF(0, y, ancho, 25), formatoCentro); y += 25;
            gfx.DrawString("Servicios y Gomas de Calidad", fontNormal, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro); y += 20;
            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 15;

            gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString($"Cajero: {cmbEmpleado.Text}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString($"Cliente: {cmbCliente.Text}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 20;

            gfx.DrawString("CANT  DESCRIPCION", fontBold, Brushes.Black, 10, y);
            gfx.DrawString("TOTAL", fontBold, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 20;

            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                string cant = fila.Cells[2].Value.ToString();
                string desc = fila.Cells[1].Value.ToString();
                decimal totalLinea = Convert.ToDecimal(fila.Cells[4].Value);

                if (desc.Length > 16) desc = desc.Substring(0, 14) + "..";

                gfx.DrawString($"{cant}x", fontNormal, Brushes.Black, 5, y);
                gfx.DrawString(desc, fontNormal, Brushes.Black, 45, y);
                gfx.DrawString(totalLinea.ToString("C2"), fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 5, 20), formatoDerecha);
                y += 20;
            }

            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 15;

            gfx.DrawString("SUBTOTAL:", fontNormal, Brushes.Black, 100, y);
            gfx.DrawString(lblSubTotal.Text, fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 15;

            gfx.DrawString("ITBIS (18%):", fontNormal, Brushes.Black, 100, y);
            gfx.DrawString(lblImpuesto.Text, fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 20;

            gfx.DrawString("TOTAL:", fontBold, Brushes.Black, 100, y);
            gfx.DrawString(lblTotal.Text, fontBold, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 30;

            gfx.DrawString("¡Gracias por su preferencia!", fontBold, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro); y += 20;
            gfx.DrawString("Conserve su recibo para garantía", fontNormal, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro);
        }
    }
}