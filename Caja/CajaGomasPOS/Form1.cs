using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace CajaGomasPOS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // CADENA DE CONEXIÓN REAL AL ARCHIVO MDF
        string conexionBD = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=\"C:\\INTEC\\Trimestre 7\\IDS345-02 DESARROLLO DE SOFTWARE III\\SistemaGomas 12-04\\wSIstemaGomas\\Caja\\CajaGomasPOS\\App_Data\\GomasDB.mdf\";Integrated Security=True";

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

        public class FacturaPOS
        {
            public string FechaHora { get; set; }
            public string Cliente { get; set; }
            public string Vendedor { get; set; }
            public string MetodoPago { get; set; }
            public string SubTotal { get; set; }
            public string ITBIS { get; set; }
            public string Total { get; set; }

            [Browsable(false)] public int IdCliente { get; set; }
            [Browsable(false)] public int IdEmpleado { get; set; }
            [Browsable(false)] public int IdSucursal { get; set; }
            [Browsable(false)] public decimal ImpuestoBD { get; set; }
            [Browsable(false)] public string EstadoFactura { get; set; }
        }

        List<FacturaPOS> historialVentas = new List<FacturaPOS>();
        List<ArticuloPOS> inventario = new List<ArticuloPOS>();

        public void ActualizarGaveta()
        {
            decimal totalEnGaveta = FondoCaja + TotalEfectivoDelDia;
            lblDineroCaja.Text = "Dinero en Gaveta: " + totalEnGaveta.ToString("C");
        }

        private void GuardarVentaOffline()
        {
            try
            {
                string fechaHora = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt");
                string textoFactura = $"FECHA: {fechaHora}\nTOTAL PAGADO: {lblTotal.Text} (Pago en {reciboMetodo})\nARTÍCULOS VENDIDOS:\n";
                foreach (DataGridViewRow fila in dgvCarrito.Rows)
                {
                    textoFactura += $"  - {fila.Cells[2].Value}x {fila.Cells[1].Value} (${fila.Cells[4].Value})\n";
                }
                textoFactura += "--------------------------------------------------\n";
                System.IO.File.AppendAllText("Backup_VentasOffline.txt", textoFactura);
            }
            catch (Exception ex) { MessageBox.Show("Advertencia: No se pudo guardar el respaldo offline. " + ex.Message); }
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
                if (pantallaLogin.RolUsuario == "Admin")
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

        private void Form1_Load(object sender, EventArgs e)
        {
            // Vinculamos el objeto de impresión con la función que acabamos de crear
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

            // CARGA DE DATOS REALES DESDE SQL
            CargarInventarioDesdeBD();
            CargarPersonasDesdeBD();

            ActualizarGaveta();
            cmbSucursal.Items.Add("Principal - Santo Domingo");
            cmbSucursal.SelectedIndex = 0;

            cmbTipoItem.Items.Add("Producto (Goma)");
            cmbTipoItem.Items.Add("Servicio (Taller)");
            cmbTipoItem.SelectedIndex = 0;
        }

        private void CargarPersonasDesdeBD()
        {
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                try
                {
                    conexion.Open();
                    // CLIENTES
                    string sqlClientes = "SELECT c.IdCliente, u.Nombres + ' ' + u.Apellidos AS Nombre FROM tblCliente c INNER JOIN tblUsuario u ON c.IdUsuario = u.IdUsuario WHERE c.Estado = 1";
                    SqlDataAdapter daCli = new SqlDataAdapter(sqlClientes, conexion);
                    DataTable dtCli = new DataTable();
                    daCli.Fill(dtCli);
                    cmbCliente.DataSource = dtCli;
                    cmbCliente.DisplayMember = "Nombre";
                    cmbCliente.ValueMember = "IdCliente";

                    // EMPLEADOS
                    string sqlEmpleados = "SELECT e.IdEmpleado, u.Nombres + ' ' + u.Apellidos AS Nombre FROM tblEmpleado e INNER JOIN tblUsuario u ON e.IdUsuario = u.IdUsuario WHERE e.Estado = 1";
                    SqlDataAdapter daEmp = new SqlDataAdapter(sqlEmpleados, conexion);
                    DataTable dtEmp = new DataTable();
                    daEmp.Fill(dtEmp);
                    cmbEmpleado.DataSource = dtEmp;
                    cmbEmpleado.DisplayMember = "Nombre";
                    cmbEmpleado.ValueMember = "IdEmpleado";
                }
                catch (Exception ex) { MessageBox.Show("Error al cargar Clientes/Empleados: " + ex.Message); }
            }
        }

        private void CargarInventarioDesdeBD()
        {
            inventario.Clear();
            int idSucursalActual = 1;
            using (SqlConnection conexion = new SqlConnection(conexionBD))
            {
                try
                {
                    conexion.Open();
                    string queryProductos = "SELECT p.IdProducto, p.Marca, p.Modelo, p.Medida, p.PrecioVenta, ISNULL(i.StockActual, 0) AS StockActual FROM tblProducto p LEFT JOIN tblInventario i ON p.IdProducto = i.IdProducto AND i.IdSucursal = @IdSucursal WHERE p.Estado = 1";
                    using (SqlCommand cmd = new SqlCommand(queryProductos, conexion))
                    {
                        cmd.Parameters.AddWithValue("@IdSucursal", idSucursalActual);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inventario.Add(new ArticuloPOS
                                {
                                    IdArticulo = Convert.ToInt32(reader["IdProducto"]),
                                    Tipo = "Goma",
                                    Marca = reader["Marca"].ToString(),
                                    Medida = reader["Medida"].ToString(),
                                    Modelo = reader["Modelo"].ToString(),
                                    Precio = Convert.ToDecimal(reader["PrecioVenta"]),
                                    Stock = Convert.ToInt32(reader["StockActual"])
                                });
                            }
                        }
                    }
                    string queryServicios = "SELECT IdServicio, NombreServicio, Precio FROM tblServicio WHERE Estado = 1";
                    using (SqlCommand cmd = new SqlCommand(queryServicios, conexion))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inventario.Add(new ArticuloPOS
                                {
                                    IdArticulo = Convert.ToInt32(reader["IdServicio"]),
                                    Tipo = "Servicio",
                                    Marca = "",
                                    Medida = "",
                                    Modelo = reader["NombreServicio"].ToString(),
                                    Precio = Convert.ToDecimal(reader["Precio"]),
                                    Stock = 999
                                });
                            }
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show("Error de inventario: " + ex.Message); }
            }
        }

        // --- LÓGICA DE SELECCIÓN (CASCADA) ---
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
            // Verificación de columnas (Tu escudo anti-error de índice)
            if (dgvCarrito.ColumnCount <= 5)
            {
                DataGridViewTextBoxColumn colId = new DataGridViewTextBoxColumn();
                colId.Name = "colIdOriginal";
                colId.Visible = false;
                dgvCarrito.Columns.Add(colId);
            }

            // --- NUEVO: AVISO SI NO HAY NADA SELECCIONADO ---
            if (cmbModelo.SelectedItem == null)
            {
                MessageBox.Show("⚠️ Por favor, seleccione un Producto o Servicio antes de intentar agregarlo al carrito.",
                                "Falta selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void btnFacturar_Click(object sender, EventArgs e)
        {
            if (dgvCarrito.Rows.Count == 0)
            {
                MessageBox.Show("🛒 El carrito está vacío. Agregue al menos un artículo para poder facturar.",
                                "Carrito Vacío", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                using (SqlConnection con = new SqlConnection(conexionBD))
                {
                    con.Open();
                    SqlTransaction trx = con.BeginTransaction();
                    try
                    {
                        string sqlF = @"INSERT INTO tblFactura (Impuesto, EstadoFactura, MetodoPago, IdCliente, IdEmpleado, IdSucursal, Estado) 
                                        VALUES (18, 'PAGADA', @met, @cli, @emp, 1, 1); SELECT SCOPE_IDENTITY();";
                        int idF = 0;
                        using (SqlCommand cmdF = new SqlCommand(sqlF, con, trx))
                        {
                            cmdF.Parameters.AddWithValue("@met", reciboMetodo);
                            cmdF.Parameters.AddWithValue("@cli", cmbCliente.SelectedValue);
                            cmdF.Parameters.AddWithValue("@emp", cmbEmpleado.SelectedValue);
                            idF = Convert.ToInt32(cmdF.ExecuteScalar());
                        }

                        foreach (DataGridViewRow f in dgvCarrito.Rows)
                        {
                            char tipo = f.Cells[0].Value.ToString().Contains("Goma") ? 'P' : 'S';
                            int idArt = Convert.ToInt32(f.Cells[5].Value);
                            int cant = Convert.ToInt32(f.Cells[2].Value);
                            decimal prec = Convert.ToDecimal(f.Cells[3].Value);

                            string sqlD = @"INSERT INTO tblDetalle_Factura (TipoItem, IdServicio, IdProducto, Cantidad, PrecioUnitario, IdFactura, IdVehiculo, Estado)
                                            VALUES (@t, @s, @p, @c, @pr, @id, 1, 1)";
                            using (SqlCommand cmdD = new SqlCommand(sqlD, con, trx))
                            {
                                cmdD.Parameters.AddWithValue("@t", tipo);
                                cmdD.Parameters.AddWithValue("@s", tipo == 'S' ? (object)idArt : DBNull.Value);
                                cmdD.Parameters.AddWithValue("@p", tipo == 'P' ? (object)idArt : DBNull.Value);
                                cmdD.Parameters.AddWithValue("@c", cant);
                                cmdD.Parameters.AddWithValue("@pr", prec);
                                cmdD.Parameters.AddWithValue("@id", idF);
                                cmdD.ExecuteNonQuery();
                            }
                            if (tipo == 'P')
                            {
                                string sqlS = "UPDATE tblInventario SET StockActual = StockActual - @c WHERE IdProducto = @p";
                                using (SqlCommand cmdS = new SqlCommand(sqlS, con, trx))
                                {
                                    cmdS.Parameters.AddWithValue("@c", cant);
                                    cmdS.Parameters.AddWithValue("@p", idArt);
                                    cmdS.ExecuteNonQuery();
                                }
                            }
                        }
                        trx.Commit();
                        if (reciboMetodo == "Efectivo") { TotalEfectivoDelDia += (reciboEfectivo - reciboDevuelta); ActualizarGaveta(); }
                        MessageBox.Show("Venta guardada!");
                        previewImprimir.ShowDialog();
                        dgvCarrito.Rows.Clear(); RecalcularTotales();
                    }
                    catch (Exception ex) { trx.Rollback(); MessageBox.Show("Error: " + ex.Message); }
                }
            }
        }

        private void btnSupervisor_Click(object sender, EventArgs e)
        {
            Form formReporte = new Form { Text = "PANEL DE CONTROL - MODO SUPERVISOR", Size = new Size(850, 500), StartPosition = FormStartPosition.CenterScreen };
            TabControl tabs = new TabControl { Dock = DockStyle.Fill };

            // --- PESTAÑA 1: INVENTARIO ---
            TabPage tabInv = new TabPage("📦 Inventario");
            DataGridView dgvInv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            dgvInv.DataSource = inventario.ToList();

            // Formatear precio en inventario si existe la columna
            if (dgvInv.Columns["Precio"] != null) dgvInv.Columns["Precio"].DefaultCellStyle.Format = "N2";
            tabInv.Controls.Add(dgvInv);

            // --- PESTAÑA 2: VENTAS SQL ---
            TabPage tabFact = new TabPage("💰 Historial de Ventas");
            DataGridView dgvFact = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(conexionBD))
            {
                try
                {
                    // Traemos los datos. Nota: El SQL a veces trae muchos decimales por el tipo de dato Money o Decimal
                    string q = "SELECT IdFactura, Fecha, MetodoPago, SubTotal, TotalGeneral FROM tblFactura WHERE Estado = 1 ORDER BY Fecha DESC";
                    SqlDataAdapter da = new SqlDataAdapter(q, con);
                    da.Fill(dt);
                }
                catch (Exception ex) { MessageBox.Show("Error SQL: " + ex.Message); }
            }

            dgvFact.DataSource = dt;

            // --- AQUÍ ESTÁ EL ARREGLO PARA LOS DECIMALES ---
            // Usamos un bucle para formatear CUALQUIER columna que sea numérica
            foreach (DataGridViewColumn col in dgvFact.Columns)
            {
                // Si la columna es SubTotal o TotalGeneral, le forzamos 2 decimales
                if (col.Name == "SubTotal" || col.Name == "TotalGeneral")
                {
                    col.DefaultCellStyle.Format = "N2"; // Esto quita los chorros de ceros
                }
            }

            // Calcular el total para el título de la pestaña
            decimal sumaTodo = 0;
            if (dt.Rows.Count > 0)
            {
                sumaTodo = dt.AsEnumerable().Sum(row => Convert.ToDecimal(row["TotalGeneral"]));
            }
            tabFact.Text = $"💰 Ventas (Total: {sumaTodo:C2})";

            tabFact.Controls.Add(dgvFact);

            tabs.TabPages.Add(tabInv);
            tabs.TabPages.Add(tabFact);
            formReporte.Controls.Add(tabs);
            formReporte.ShowDialog();
        }

        // =======================================================================
        // CIERRE DE CAJA Y CUADRE OBLIGATORIO (Blindado)
        // =======================================================================
        // =======================================================================
        // CIERRE DE CAJA Y CUADRE OBLIGATORIO (MODO SEGURO)
        // =======================================================================
        private void btnCierreCaja_Click(object sender, EventArgs e)
        {
            // 1. Si NO hay ventas, le permitimos salir sin cuadrar
            if (TotalEfectivoDelDia == 0)
            {
                if (MessageBox.Show("No se registraron ventas hoy. ¿Desea salir del sistema?", "Cierre", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    Application.Exit();
                }
                return;
            }

            // 2. Si SÍ hay ventas, creamos la ventana de Cuadre Obligatorio
            Form formCuadre = new Form() { Width = 380, Height = 350, Text = "Cuadre de Caja Obligatorio", StartPosition = FormStartPosition.CenterScreen, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };

            Label lblInstruccion = new Label() { Left = 20, Top = 20, Text = "⚠️ SE REGISTRARON VENTAS EN EFECTIVO.\nIngrese el monto físico contado en gaveta:", AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold), ForeColor = Color.DarkRed };
            TextBox txtContado = new TextBox() { Left = 20, Top = 60, Width = 150, Font = new Font("Arial", 14) };
            Button btnVerificar = new Button() { Text = "Verificar Dinero", Left = 180, Top = 58, Width = 150, Height = 30, Cursor = Cursors.Hand };

            Label lblResultado = new Label() { Left = 20, Top = 110, AutoSize = true, Font = new Font("Arial", 10, FontStyle.Bold), Text = "Esperando verificación..." };

            // El botón de salida empieza APAGADO y con color gris
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
                        // --- CASO 1: CUADRE PERFECTO ---
                        lblResultado.ForeColor = Color.DarkGreen;
                        lblResultado.Text = $"Efectivo Esperado: {totalEsperado:C}\nFísico Contado: {efectivoContado:C}\n\n✅ CUADRE PERFECTO.\nYa puede cerrar el sistema.";

                        btnCerrarTurno.Enabled = true; // SE ACTIVA EL BOTÓN
                        btnCerrarTurno.BackColor = Color.LimeGreen; // Color de éxito
                        txtContado.ReadOnly = true; // Bloqueamos el campo para que no lo altere
                        btnVerificar.Enabled = false;
                    }
                    else
                    {
                        // --- CASO 2: HAY DIFERENCIA (SOBRANTE O FALTANTE) ---
                        string mensajeDiferencia = diferencia > 0 ? $"SOBRANTE: {diferencia:C}" : $"FALTANTE: {Math.Abs(diferencia):C}";

                        lblResultado.ForeColor = Color.Crimson;
                        lblResultado.Text = $"Efectivo Esperado: {totalEsperado:C}\nFísico Contado: {efectivoContado:C}\n\n❌ {mensajeDiferencia}\n\nERROR: El dinero no coincide.\nNo se permite el cierre hasta cuadrar.";

                        btnCerrarTurno.Enabled = false; // SE MANTIENE BLOQUEADO
                        btnCerrarTurno.BackColor = Color.LightGray;
                        txtContado.Focus();
                        txtContado.SelectAll();
                    }
                }
                catch { MessageBox.Show("Por favor, ingrese un monto numérico válido.", "Error de formato"); }
            };

            btnCerrarTurno.Click += (s, ev) =>
            {
                MessageBox.Show("Turno cerrado correctamente. Vuelva pronto.", "Cierre Exitoso");
                Application.Exit();
            };

            // Agregamos controles al mini-formulario
            formCuadre.Controls.Add(lblInstruccion);
            formCuadre.Controls.Add(txtContado);
            formCuadre.Controls.Add(btnVerificar);
            formCuadre.Controls.Add(lblResultado);
            formCuadre.Controls.Add(btnCerrarTurno);

            formCuadre.ShowDialog();
        }
        // =======================================================================
        // GENERADOR DE TICKET DE VENTA (DISEÑO PROFESIONAL PARA IMPRESORA TÉRMICA)
        // =======================================================================
        private void docImprimir_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // 1. Herramientas de dibujo
            Graphics gfx = e.Graphics;
            Font fontTitulo = new Font("Courier New", 14, FontStyle.Bold);
            Font fontNormal = new Font("Courier New", 10);
            Font fontBold = new Font("Courier New", 10, FontStyle.Bold);

            StringFormat formatoCentro = new StringFormat { Alignment = StringAlignment.Center };
            StringFormat formatoDerecha = new StringFormat { Alignment = StringAlignment.Far };

            int y = 20; // Margen superior
            int ancho = 300; // Ancho estándar de papel térmico (80mm)
            string separador = "--------------------------------------";

            // 2. Encabezado del Taller
            gfx.DrawString("PRECISION TIRE POS", fontTitulo, Brushes.Black, new RectangleF(0, y, ancho, 25), formatoCentro);
            y += 25;
            gfx.DrawString("Servicios y Gomas de Calidad", fontNormal, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro);
            y += 20;
            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 15;

            // 3. Info de la Factura
            gfx.DrawString($"Fecha: {DateTime.Now:dd/MM/yyyy HH:mm}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString($"Cajero: {cmbEmpleado.Text}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString($"Cliente: {cmbCliente.Text}", fontNormal, Brushes.Black, 10, y); y += 15;
            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 20;

            // 4. Cabecera de tabla
            gfx.DrawString("CANT  DESCRIPCION", fontBold, Brushes.Black, 10, y);
            gfx.DrawString("TOTAL", fontBold, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha);
            y += 20;

            // 5. Detalles del Carrito (VERSIÓN CORREGIDA)
            foreach (DataGridViewRow fila in dgvCarrito.Rows)
            {
                string cant = fila.Cells[2].Value.ToString();
                string desc = fila.Cells[1].Value.ToString();
                decimal totalLinea = Convert.ToDecimal(fila.Cells[4].Value);

                // Ajustamos el largo máximo de la descripción para que no choque con el precio
                // Bajamos de 20 a 16 caracteres para dejar aire al símbolo de $
                if (desc.Length > 16) desc = desc.Substring(0, 14) + "..";

                // Dibujamos la cantidad y la descripción con un poco más de margen a la izquierda
                gfx.DrawString($"{cant}x", fontNormal, Brushes.Black, 5, y);
                gfx.DrawString(desc, fontNormal, Brushes.Black, 45, y); // Movimos la descripción a la posición 45

                // Dibujamos el precio alineado a la derecha
                gfx.DrawString(totalLinea.ToString("C2"), fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 5, 20), formatoDerecha);

                y += 20;
            }

            // 6. Totales
            gfx.DrawString(separador, fontNormal, Brushes.Black, 0, y); y += 15;

            gfx.DrawString("SUBTOTAL:", fontNormal, Brushes.Black, 100, y);
            gfx.DrawString(lblSubTotal.Text, fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 15;

            gfx.DrawString("ITBIS (18%):", fontNormal, Brushes.Black, 100, y);
            gfx.DrawString(lblImpuesto.Text, fontNormal, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 20;

            gfx.DrawString("TOTAL:", fontBold, Brushes.Black, 100, y);
            gfx.DrawString(lblTotal.Text, fontBold, Brushes.Black, new RectangleF(0, y, ancho - 10, 20), formatoDerecha); y += 30;

            // 7. Pie de página
            gfx.DrawString("¡Gracias por su preferencia!", fontBold, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro);
            y += 20;
            gfx.DrawString("Conserve su recibo para garantía", fontNormal, Brushes.Black, new RectangleF(0, y, ancho, 20), formatoCentro);
        }
    }
}