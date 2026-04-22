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
using System.Configuration;

namespace CajaGomasPOS
{
    public partial class Form1 : Form
    {
        private string UrlIntegracion = ConfigurationManager.AppSettings["UrlIntegracion"];
        public Form1()
        {
            InitializeComponent();
        }

        string reciboMetodo = "";
        decimal reciboEfectivo = 0;
        decimal reciboDevuelta = 0;

        public decimal FondoCaja = 0;
        public ToolStripLabel lblEstadoCaja; // Nuestro semáforo de la caja
        public decimal TotalEfectivoDelDia = 0;
        public bool ModoSupervisor = false;
        public MenuStrip MenuSuperior;
        public ToolStripMenuItem menuAdmin;
        public bool estaLogueado = false;

        // --- NUEVAS VARIABLES DE CONTROL ---
        public decimal FondoInicial = 0;
        public decimal FondoExtra = 0;
        public bool CajaAbierta = false; // El candado principal del sistema

        // Lista para guardar todo lo que se venda en el día
        public class ArticuloVendido
        {
            public string Descripcion { get; set; }
            public int Cantidad { get; set; }
            public decimal Total { get; set; }
        }
        public List<ArticuloVendido> ResumenVentasDia = new List<ArticuloVendido>();

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
            // --- NUEVO: EL INDICADOR DE ESTADO EN EL MENÚ ---
            lblEstadoCaja = new ToolStripLabel("🔴 CAJA CERRADA (Requiere Apertura)");
            lblEstadoCaja.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblEstadoCaja.ForeColor = Color.Crimson;
            lblEstadoCaja.Alignment = ToolStripItemAlignment.Right; // Lo empuja a la esquina derecha
            MenuSuperior.Items.Add(lblEstadoCaja);
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
                    
                }
                else
                {
                    ModoSupervisor = false;
                    menuAdmin.Visible = false;
                    
                }
            }
            else if (estaLogueado == false)
            {
                Environment.Exit(0);
            }
        }
        private void AbrirCaja()
        {
            // Guardamos el estado actual ANTES de modificarlo para que los mensajes salgan bien
            bool esApertura = !CajaAbierta;

            Form formFondo = new Form() { Width = 300, Height = 150, Text = esApertura ? "Apertura de Caja" : "Agregar Fondo Extra", StartPosition = FormStartPosition.CenterScreen, FormBorderStyle = FormBorderStyle.FixedDialog };
            Label lblTexto = new Label() { Left = 20, Top = 20, Text = esApertura ? "Ingrese el fondo INICIAL ($):" : "Ingrese monto EXTRA ($):", AutoSize = true };
            TextBox txtFondo = new TextBox() { Left = 20, Top = 50, Width = 150 };
            Button btnAceptar = new Button() { Text = esApertura ? "Abrir Caja" : "Agregar", Left = 180, Top = 48, Width = 80 };

            btnAceptar.Click += (sender, e) => {
                try
                {
                    decimal montoIngresado = Convert.ToDecimal(txtFondo.Text);

                    if (esApertura)
                    {
                        FondoInicial = montoIngresado;
                        CajaAbierta = true;

                        // Cambiamos el semáforo (verificando que exista)
                        if (lblEstadoCaja != null)
                        {
                            lblEstadoCaja.Text = "🟢 CAJA ABIERTA";
                            lblEstadoCaja.ForeColor = Color.DarkGreen;
                        }
                    }
                    else
                    {
                        FondoExtra += montoIngresado;
                    }

                    FondoCaja = FondoInicial + FondoExtra;
                    ActualizarGaveta();

                    MessageBox.Show(esApertura ? "Caja abierta exitosamente." : "Fondo extra agregado a la gaveta.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    formFondo.Close();

                    // 🛠️ EL ARREGLO ESTÁ AQUÍ: Ahora solo revisa los botones que realmente son menús desplegables
                    foreach (ToolStripItem item in MenuSuperior.Items)
                    {
                        if (item is ToolStripMenuItem menuItem && menuItem.Text == "💰 Operaciones de Caja")
                        {
                            var submenu = (ToolStripMenuItem)menuItem.DropDownItems[0];
                            submenu.Text = "Agregar Fondo Extra";
                        }
                    }
                }
                catch (FormatException)
                {
                    // Esto AHORA SÍ solo saltará si pones letras en vez de números
                    MessageBox.Show("Ingrese un monto numérico válido.", "Error de Formato", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                catch (Exception ex)
                {
                    // Si llega a pasar otra cosa rara, te dirá el error real y no "por la cara"
                    MessageBox.Show("Ocurrió un error inesperado: " + ex.Message, "Error Interno", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
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
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);

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
            bool hayConexion = false;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);
                    client.Timeout = TimeSpan.FromSeconds(3); // Solo esperamos 3 segundos

                    // 1. Traer los neumáticos del catálogo
                    var resGomas = await client.GetAsync($"api/productos/catalogo/{idSucursalActual}");
                    if (resGomas.IsSuccessStatusCode)
                    {
                        var jsonGomas = await resGomas.Content.ReadAsStringAsync();
                        var gomas = JsonConvert.DeserializeObject<List<dynamic>>(jsonGomas);
                        foreach (var g in gomas)
                        {
                            inventario.Add(new ArticuloPOS { IdArticulo = (int)g.IdProducto, Tipo = "Goma", Marca = (string)g.Marca, Medida = (string)g.Medida, Modelo = (string)g.Modelo, Precio = (decimal)g.PrecioVenta, Stock = (int)g.StockActual });
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
                            inventario.Add(new ArticuloPOS { IdArticulo = (int)s.IdServicio, Tipo = "Servicio", Marca = "", Medida = "", Modelo = (string)s.NombreServicio, Precio = (decimal)s.Precio, Stock = 999 });
                        }
                    }

                    // SI LLEGAMOS AQUÍ, HAY INTERNET. Guardamos el inventario en el disco duro para usarlo mañana si no hay red.
                    hayConexion = true;
                    string inventarioCaché = JsonConvert.SerializeObject(inventario);
                    System.IO.File.WriteAllText("InventarioLocal.json", inventarioCaché);
                }
            }
            catch (Exception)
            {
                hayConexion = false; // Falló la red
            }

            // 3. MODO OFFLINE: Si la red falló, cargamos el inventario del disco duro
            if (!hayConexion)
            {
                if (System.IO.File.Exists("InventarioLocal.json"))
                {
                    string cache = System.IO.File.ReadAllText("InventarioLocal.json");
                    inventario = JsonConvert.DeserializeObject<List<ArticuloPOS>>(cache);
                    MessageBox.Show("Sin conexión al Servidor. El Inventario se cargó en Modo Offline.", "Aviso Local", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No hay conexión y no existe un caché previo. El inventario está vacío.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
            // --- EL CANDADO DE APERTURA ---
            if (!CajaAbierta)
            {
                MessageBox.Show("⚠️ CAJA CERRADA.\nDebe realizar la Apertura de Caja (ingresar fondo inicial) antes de poder registrar ventas.", "Acceso Denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Esto detiene el proceso y no agrega nada al carrito
            }

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

            // 1. Abrimos la pantalla de cobro (FormCobro)
            FormCobro cobro = new FormCobro();
            cobro.TotalPagar = subTotal * 1.18m;
            cobro.EfectivoEnCaja = FondoCaja + TotalEfectivoDelDia;

            if (cobro.ShowDialog() == DialogResult.OK)
            {
                reciboMetodo = cobro.MetodoPago;
                reciboEfectivo = cobro.EfectivoEntregado;
                reciboDevuelta = cobro.CambioDevuelto;

                // 2. Preparamos el paquete de datos con lo que el cliente compró
                var peticionVenta = new
                {
                    IdCliente = Convert.ToInt32(cmbCliente.SelectedValue),
                    IdEmpleado = Convert.ToInt32(cmbEmpleado.SelectedValue),
                    IdSucursal = 1,
                    IdVehiculo = 1,
                    MetodoPago = reciboMetodo,
                    TotalGeneral = cobro.TotalPagar, // Guardamos el total para el modo offline
                    Fecha = DateTime.Now,            // Guardamos la fecha exacta para el modo offline
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

                // Variable bandera para saber si guardamos el desglose al final
                bool ventaCompletada = false;

                // 3. INTENTO ONLINE: Tratamos de mandarlo al servidor central
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(UrlIntegracion);
                        var content = new StringContent(JsonConvert.SerializeObject(peticionVenta), Encoding.UTF8, "application/json");

                        var response = await client.PostAsync("api/facturacion/procesar", content);

                        if (response.IsSuccessStatusCode)
                        {
                            // ¡ÉXITO ONLINE!
                            if (reciboMetodo == "Efectivo") { TotalEfectivoDelDia += (reciboEfectivo - reciboDevuelta); ActualizarGaveta(); }
                            MessageBox.Show("✅ ¡Venta procesada y guardada exitosamente en la base central!");

                            ventaCompletada = true; // Confirmamos que se hizo la venta
                        }
                        else
                        {
                            string error = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("❌ El Servidor rechazó la venta:\n" + error, "Error de validación", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                catch (Exception)
                {
                    // 4. INTENTO OFFLINE: ¡Si el servidor está apagado o no hay internet!
                    GestorOffline.GuardarFacturaLocal(peticionVenta);

                    if (reciboMetodo == "Efectivo")
                    {
                        TotalEfectivoDelDia += (reciboEfectivo - reciboDevuelta);
                        ActualizarGaveta();
                    }

                    MessageBox.Show("Fallo de red detectado.\n\nLa venta fue completada y guardada en MODO OFFLINE. Recuerde sincronizar más tarde.", "Modo Offline", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    ventaCompletada = true; // Confirmamos que se hizo la venta localmente
                }


                // ==============================================================
                // 5. SI LA VENTA SE COMPLETÓ (ONLINE U OFFLINE), HACEMOS ESTO:
                // ==============================================================
                if (ventaCompletada)
                {
                    // Imprimimos el recibo al cliente
                    previewImprimir.ShowDialog();

                    // --- GUARDAR DESGLOSE DEL DÍA PARA EL CUADRE ---
                    foreach (DataGridViewRow f in dgvCarrito.Rows)
                    {
                        string desc = f.Cells[1].Value.ToString();
                        int cant = Convert.ToInt32(f.Cells[2].Value);
                        decimal totalLinea = Convert.ToDecimal(f.Cells[4].Value);

                        var existente = ResumenVentasDia.FirstOrDefault(x => x.Descripcion == desc);
                        if (existente != null)
                        {
                            existente.Cantidad += cant;
                            existente.Total += totalLinea;
                        }
                        else
                        {
                            ResumenVentasDia.Add(new ArticuloVendido { Descripcion = desc, Cantidad = cant, Total = totalLinea });
                        }
                    }
                    // -----------------------------------------------

                    // Limpiamos el carrito para el próximo cliente
                    dgvCarrito.Rows.Clear();
                    RecalcularTotales();

                    // Actualizamos inventario visualmente (si hay red, baja datos nuevos; si no, ignora)
                    await CargarInventarioDesdeAPI();
                }
            }
        }
        private async void btnSupervisor_Click(object sender, EventArgs e)
        {
            Form formReporte = new Form { Text = "PANEL DE CONTROL - MODO SUPERVISOR", Size = new Size(850, 500), StartPosition = FormStartPosition.CenterScreen };
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            // --- PESTAÑA 1: INVENTARIO ---
            TabPage tabInv = new TabPage("📦 Inventario Local (Cacheado)");
            DataGridView dgvInv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvInv.DataSource = inventario.ToList();
            if (dgvInv.Columns["Precio"] != null) dgvInv.Columns["Precio"].DefaultCellStyle.Format = "N2";
            tabInv.Controls.Add(dgvInv);

            // --- PESTAÑA 2: VENTAS EN VIVO ---
            TabPage tabFact = new TabPage("💰 Ventas del Día (En Vivo)");
            DataGridView dgvFact = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            decimal sumaTodo = 0;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);
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


            // ==========================================================
            // --- NUEVA PESTAÑA 3: SINCRONIZACIÓN OFFLINE ---
            // ==========================================================
            TabPage tabSync = new TabPage("🔄 Sincronización");
            tabSync.BackColor = Color.White;

            int numPendientes = GestorOffline.ContarPendientes();
            Label lblAviso = new Label
            {
                Text = $"Facturas en Modo Offline pendientes de subir: {numPendientes}",
                AutoSize = true,
                Location = new Point(30, 40),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = numPendientes > 0 ? Color.DarkRed : Color.Black
            };

            Button btnSincronizar = new Button
            {
                Text = "🚀 SINCRONIZAR DATOS AHORA",
                Size = new Size(350, 60),
                Location = new Point(30, 90),
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatStyle = FlatStyle.Flat
            };
            btnSincronizar.FlatAppearance.BorderSize = 0;

            // Acción del botón
            btnSincronizar.Click += (s, ev) =>
            {
                EjecutarSincronizacion(); // Llama al método que creamos antes

                // Refresca la etiqueta después de intentar sincronizar
                int nuevosPendientes = GestorOffline.ContarPendientes();
                lblAviso.Text = $"Facturas en Modo Offline pendientes de subir: {nuevosPendientes}";
                lblAviso.ForeColor = nuevosPendientes > 0 ? Color.DarkRed : Color.Black;
            };

            tabSync.Controls.Add(lblAviso);
            tabSync.Controls.Add(btnSincronizar);
            // ==========================================================


            // --- AGREGAMOS LAS 3 PESTAÑAS AL PANEL ---
            tabs.TabPages.Add(tabInv);
            tabs.TabPages.Add(tabFact);
            tabs.TabPages.Add(tabSync); // <-- Aquí está la nueva

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

            // Modifica esta partecita dentro de tu btnCierreCaja_Click:
            btnCerrarTurno.Click += (s, ev) =>
            {
                ImprimirTicketCuadre(); // <--- AGREGAMOS LA IMPRESIÓN AQUÍ
                MessageBox.Show("Turno cerrado correctamente. Vuelva pronto.", "Cierre Exitoso");
                Application.Exit();
            };

            formCuadre.Controls.Add(lblInstruccion); formCuadre.Controls.Add(txtContado); formCuadre.Controls.Add(btnVerificar); formCuadre.Controls.Add(lblResultado); formCuadre.Controls.Add(btnCerrarTurno);
            formCuadre.ShowDialog();
        }
        private void ImprimirTicketCuadre()
        {
            System.Drawing.Printing.PrintDocument docCuadre = new System.Drawing.Printing.PrintDocument();

            docCuadre.PrintPage += (s, ev) =>
            {
                Font fontTitulo = new Font("Courier New", 14, FontStyle.Bold);
                Font fontNormal = new Font("Courier New", 10);
                Font fontPequeña = new Font("Courier New", 8);
                int y = 20;

                ev.Graphics.DrawString("=== CIERRE DE CAJA ===", fontTitulo, Brushes.Black, 20, y); y += 30;
                ev.Graphics.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", fontNormal, Brushes.Black, 10, y); y += 20;
                ev.Graphics.DrawString($"Cajero: {cmbEmpleado.Text}", fontNormal, Brushes.Black, 10, y); y += 30;

                // 1. EL DINERO DE LA CAJA DESGLOSADO
                ev.Graphics.DrawString("--- FLUJO DE EFECTIVO ---", fontNormal, Brushes.Black, 10, y); y += 20;
                ev.Graphics.DrawString($"Fondo INICIAL:    {FondoInicial:C2}", fontNormal, Brushes.Black, 10, y); y += 20;
                if (FondoExtra > 0)
                {
                    ev.Graphics.DrawString($"Fondo EXTRA:      {FondoExtra:C2}", fontNormal, Brushes.Black, 10, y); y += 20;
                }
                ev.Graphics.DrawString($"Ventas Efectivo:  {TotalEfectivoDelDia:C2}", fontNormal, Brushes.Black, 10, y); y += 20;

                ev.Graphics.DrawString("-------------------------", fontNormal, Brushes.Black, 10, y); y += 20;
                decimal totalGaveta = FondoInicial + FondoExtra + TotalEfectivoDelDia;
                ev.Graphics.DrawString($"TOTAL EN GAVETA:  {totalGaveta:C2}", fontTitulo, Brushes.Black, 10, y); y += 40;

                // 2. EL DESGLOSE DE ARTÍCULOS VENDIDOS
                ev.Graphics.DrawString("--- ARTICULOS VENDIDOS HOY ---", fontNormal, Brushes.Black, 10, y); y += 20;
                if (ResumenVentasDia.Count == 0)
                {
                    ev.Graphics.DrawString("(No hubo ventas registradas)", fontPequeña, Brushes.Black, 10, y); y += 20;
                }
                else
                {
                    foreach (var item in ResumenVentasDia)
                    {
                        string desc = item.Descripcion.Length > 20 ? item.Descripcion.Substring(0, 18) + ".." : item.Descripcion;
                        ev.Graphics.DrawString($"{item.Cantidad}x {desc}", fontPequeña, Brushes.Black, 10, y);
                        ev.Graphics.DrawString($"{item.Total:C2}", fontPequeña, Brushes.Black, 220, y);
                        y += 15;
                    }
                }

                y += 30;
                ev.Graphics.DrawString("Firma Cajero: __________________", fontNormal, Brushes.Black, 10, y); y += 30;
                ev.Graphics.DrawString("Firma Superv: __________________", fontNormal, Brushes.Black, 10, y);
            };

            PrintPreviewDialog preview = new PrintPreviewDialog { Document = docCuadre };
            preview.ShowDialog();
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
        private async void EjecutarSincronizacion()
        {
            int pendientes = GestorOffline.ContarPendientes();
            if (pendientes == 0)
            {
                MessageBox.Show("No hay datos offline pendientes por sincronizar.", "Todo al día", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var facturasOffline = GestorOffline.LeerFacturasLocales();
            int sincronizadas = 0;
            int fallidas = 0;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(UrlIntegracion);

                    foreach (dynamic factura in facturasOffline)
                    {
                        try
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(factura), Encoding.UTF8, "application/json");
                            var response = await client.PostAsync("api/Facturacion/Procesar", content);

                            if (response.IsSuccessStatusCode)
                                sincronizadas++;
                            else
                                fallidas++;
                        }
                        catch { fallidas++; }
                    }
                }

                if (fallidas == 0)
                {
                    GestorOffline.LimpiarFacturasSincronizadas();
                    MessageBox.Show($"¡Sincronización Exitosa!\nSe subieron {sincronizadas} facturas.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Sincronizadas: {sincronizadas} | Fallidas: {fallidas}", "Resultado parcial", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al sincronizar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}