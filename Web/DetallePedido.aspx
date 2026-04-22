<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetallePedido.aspx.cs" Inherits="WebGomas.DetallePedido" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Detalle del Pedido — PrecisionTire</title>
    <style>

        :root {
            --azul:        #007BFF;
            --azul-oscuro: #0062cc;
            --azul-claro:  #e8f0fe;
            --fondo:       #F8F9FA;
            --blanco:      #ffffff;
            --gris-suave:  #f1f3f5;
            --gris-borde:  #e9ecef;
            --gris-texto:  #6c757d;
            --negro:       #1E293B;
            --verde:       #1e8449;
            --verde-claro: #d5f5e3;
            --naranja:     #d35400;
            --sombra:      0 4px 24px rgba(0,0,0,0.07);
            --radio:       16px;
        }

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: var(--fondo);
            color: var(--negro);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }

        .header {
            background: var(--blanco);
            border-bottom: 1px solid var(--gris-borde);
            padding: 0 40px;
            height: 64px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            position: sticky;
            top: 0;
            z-index: 100;
            box-shadow: 0 1px 8px rgba(0,0,0,0.05);
        }

        .header-logo { font-size: 20px; font-weight: 800; color: var(--negro); text-decoration: none; flex-shrink: 0; }
        .header-logo span { color: var(--azul); }
        .header-nav { display: flex; align-items: center; gap: 4px; }
        .header-nav a { color: var(--gris-texto); text-decoration: none; font-size: 14px; font-weight: 500; padding: 6px 14px; border-radius: 8px; transition: color 0.2s, background 0.2s; }
        .header-nav a:hover  { color: var(--azul); background: var(--azul-claro); }
        .header-nav a.activo { color: var(--azul); font-weight: 700; background: var(--azul-claro); }
        .header-user { display: flex; align-items: center; flex-shrink: 0; }
        .user-card { display: flex; align-items: center; gap: 10px; background: var(--gris-suave); border: 1px solid var(--gris-borde); border-radius: 12px; padding: 6px 6px 6px 12px; }
        .user-avatar { width: 32px; height: 32px; border-radius: 50%; background: var(--azul); color: var(--blanco); display: flex; align-items: center; justify-content: center; font-size: 14px; font-weight: 700; }
        .user-datos { display: flex; flex-direction: column; line-height: 1.3; }
        .user-saludo { font-size: 10px; font-weight: 600; color: var(--gris-texto); text-transform: uppercase; letter-spacing: 0.5px; }
        .user-nombre { font-size: 13px; font-weight: 700; color: var(--negro); }
        .btn-logout { width: 32px; height: 32px; border-radius: 8px; background: #fff5f5; border: 1px solid #fecaca; color: #e53e3e; display: flex; align-items: center; justify-content: center; text-decoration: none; font-size: 16px; transition: background 0.2s; }
        .btn-logout:hover { background: #e53e3e; color: var(--blanco); border-color: #e53e3e; }
        .btn-login-header { display: inline-flex; align-items: center; gap: 6px; padding: 8px 16px; background: var(--azul); color: var(--blanco); text-decoration: none; font-size: 13px; font-weight: 600; border-radius: 10px; }

        @media (max-width: 768px) {
            .header { padding: 0 16px; }
            .header-nav { display: none; }
            .user-datos { display: none; }
        }

        .pagina {
            flex: 1;
            max-width: 820px;
            width: 100%;
            margin: 0 auto;
            padding: 44px 24px 64px;
            animation: fadeUp 0.4s ease both;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(16px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .link-volver {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            color: var(--gris-texto);
            text-decoration: none;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 28px;
            transition: color 0.2s;
        }

        .link-volver:hover { color: var(--azul); }
        .link-volver::before { content: '←'; font-size: 16px; }

        .card-factura {
            background: var(--blanco);
            border-radius: var(--radio);
            box-shadow: var(--sombra);
            overflow: hidden;
            margin-bottom: 24px;
        }

        .card-cabecera {
            background: linear-gradient(135deg, #0f2545 0%, #1a3a6e 60%, #1565c0 100%);
            padding: 24px 32px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-cabecera h2 { color: var(--blanco); font-size: 18px; font-weight: 700; margin-bottom: 3px; }
        .card-cabecera p  { color: rgba(255,255,255,0.6); font-size: 13px; }

        .cabecera-icono {
            width: 44px;
            height: 44px;
            border-radius: 50%;
            background: rgba(255,255,255,0.12);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 20px;
        }

        .card-cuerpo { padding: 28px 32px; }

        .resumen-grid {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 16px;
            margin-bottom: 28px;
        }

        .resumen-item {
            background: var(--gris-suave);
            border-radius: 12px;
            padding: 16px;
            text-align: center;
        }

        .resumen-item-label {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            color: var(--gris-texto);
            margin-bottom: 6px;
        }

        .resumen-item-valor { font-size: 16px; font-weight: 700; color: var(--negro); }
        .resumen-item-valor.azul { color: var(--azul); font-size: 20px; }

        .seccion-titulo {
            font-size: 12px;
            font-weight: 700;
            letter-spacing: 1.5px;
            text-transform: uppercase;
            color: var(--gris-texto);
            margin-bottom: 16px;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--gris-borde);
        }

        .tabla-productos {
            width: 100%;
            border-collapse: collapse;
            margin-bottom: 24px;
        }

        .tabla-productos thead tr th {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            color: var(--gris-texto);
            padding: 10px 12px;
            text-align: left;
            background: var(--gris-suave);
            border-bottom: 2px solid var(--gris-borde);
        }

        .tabla-productos thead tr th:last-child { text-align: right; }

        .tabla-productos tbody tr {
            border-bottom: 1px solid var(--gris-suave);
            transition: background 0.15s;
        }

        .tabla-productos tbody tr:last-child { border-bottom: none; }
        .tabla-productos tbody tr:hover { background: #f8faff; }

        .tabla-productos tbody tr td {
            padding: 14px 12px;
            font-size: 14px;
            color: var(--negro);
            vertical-align: middle;
        }

        .tabla-productos tbody tr td:last-child { text-align: right; font-weight: 700; }

        .producto-nombre { font-weight: 700; font-size: 14px; margin-bottom: 3px; }
        .producto-medida { font-size: 12px; color: var(--gris-texto); }

        .seccion-total {
            background: var(--gris-suave);
            border-radius: 12px;
            padding: 18px 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 8px;
        }

        .total-label { font-size: 14px; font-weight: 600; color: var(--gris-texto); }
        .total-valor { font-size: 28px; font-weight: 800; color: var(--azul); letter-spacing: -0.5px; }

        .badge-estado { display: inline-flex; align-items: center; gap: 5px; padding: 5px 14px; border-radius: 20px; font-size: 12px; font-weight: 700; }
        .estado-pagada     { background: var(--verde-claro); color: var(--verde);   border: 1px solid #a9dfbf; }
        .estado-completado { background: var(--verde-claro); color: var(--verde);   border: 1px solid #a9dfbf; }
        .estado-procesando { background: var(--azul-claro);  color: var(--azul);    border: 1px solid #b3d4ff; }
        .estado-pendiente  { background: #fef3e2;            color: var(--naranja); border: 1px solid #f8c471; }

        .btn-volver {
            display: block;
            width: 100%;
            padding: 15px;
            text-align: center;
            background: var(--azul);
            color: var(--blanco);
            text-decoration: none;
            font-size: 15px;
            font-weight: 700;
            border-radius: 12px;
            transition: background 0.2s, transform 0.15s;
            box-shadow: 0 4px 14px rgba(0,123,255,0.30);
        }

        .btn-volver:hover { background: var(--azul-oscuro); transform: translateY(-2px); }

        .panel-error {
            background: #fff5f5;
            border: 1px solid #fed7d7;
            color: #c53030;
            padding: 20px 24px;
            border-radius: var(--radio);
            font-size: 15px;
            font-weight: 500;
        }

        .page-footer {
            text-align: center;
            padding: 28px;
            color: var(--gris-texto);
            font-size: 13px;
            border-top: 1px solid var(--gris-borde);
        }

        @media (max-width: 600px) {
            .resumen-grid { grid-template-columns: 1fr; }
            .card-cuerpo  { padding: 20px; }
            .card-cabecera { padding: 20px; }
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <header class="header">
            <a href="Productos.aspx" class="header-logo">Precision<span>Tire</span></a>
            <nav class="header-nav">
                <a href="Productos.aspx">Catálogo</a>
                <a href="Carrito.aspx">Carrito</a>
                <a href="Historial.aspx" class="activo">Mis pedidos</a>
            </nav>
            <div class="header-user">
                <asp:PlaceHolder ID="phNoLogueado" runat="server">
                    <a href="Login.aspx" class="btn-login-header">🔐 Iniciar sesión</a>
                </asp:PlaceHolder>
                <asp:PlaceHolder ID="phLogueado" runat="server" Visible="false">
                    <div class="user-card">
                        <div class="user-avatar">
                            <asp:Label ID="lblAvatar" runat="server" />
                        </div>
                        <div class="user-datos">
                            <span class="user-saludo">Bienvenido</span>
                            <asp:Label ID="lblUsuario" runat="server" CssClass="user-nombre" />
                        </div>
                        <a href="Logout.aspx" class="btn-logout" title="Cerrar sesión">⏻</a>
                    </div>
                </asp:PlaceHolder>
            </div>
        </header>

        <main class="pagina">

            <a href="Historial.aspx" class="link-volver">Volver al historial</a>

            <asp:Panel ID="pnlDetalle" runat="server" Visible="false">
                <div class="card-factura">

                    <div class="card-cabecera">
                        <div>
                            <h2>Factura <asp:Label ID="lblId" runat="server" /></h2>
                            <p>Detalle completo de tu compra</p>
                        </div>
                        <div class="cabecera-icono">🧾</div>
                    </div>

                    <div class="card-cuerpo">

                        <div class="resumen-grid">
                            <div class="resumen-item">
                                <div class="resumen-item-label">Nº Factura</div>
                                <div class="resumen-item-valor">
                                    <asp:Label ID="lblIdDetalle" runat="server" />
                                </div>
                            </div>
                            <div class="resumen-item">
                                <div class="resumen-item-label">Estado</div>
                                <div class="resumen-item-valor">
                                    <asp:Label ID="lblEstado" runat="server" />
                                </div>
                            </div>
                            <div class="resumen-item">
                                <div class="resumen-item-label">Total pagado</div>
                                <div class="resumen-item-valor azul">
                                    <asp:Label ID="lblTotal" runat="server" />
                                </div>
                            </div>
                        </div>

                        <div class="seccion-titulo">Productos comprados</div>

                        <asp:GridView
                            ID="gvProductos"
                            runat="server"
                            AutoGenerateColumns="false"
                            CssClass="tabla-productos"
                            GridLines="None">
                            <Columns>
                                <asp:TemplateField HeaderText="Producto">
                                    <ItemTemplate>
                                        <div class="producto-nombre"><%# Eval("Marca") %> <%# Eval("Modelo") %></div>
                                        <div class="producto-medida"><%# Eval("Medida") %></div>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Cantidad"       HeaderText="Cant."        />
                                <asp:BoundField DataField="PrecioUnitario" HeaderText="Precio unit." DataFormatString="{0:C2}" />
                                <asp:BoundField DataField="SubTotal"       HeaderText="Subtotal"     DataFormatString="{0:C2}" />
                            </Columns>
                        </asp:GridView>

                        <!-- Desglose ITBIS -->
                        <div class="seccion-total" style="flex-direction:column; align-items:stretch; gap:10px;">
                            <div style="display:flex; justify-content:space-between; font-size:14px; color:#6c757d; padding:6px 0;">
                                <span>Subtotal sin ITBIS</span>
                                <asp:Label ID="lblSubtotalSinItbis" runat="server" />
                            </div>
                            <div style="display:flex; justify-content:space-between; font-size:14px; color:#6c757d; padding:6px 0; border-bottom:1px solid #e9ecef;">
                                <span>ITBIS (18%)</span>
                                <asp:Label ID="lblItbis" runat="server" />
                            </div>
                            <div style="display:flex; justify-content:space-between; align-items:center; padding-top:6px;">
                                <span class="total-label">Total general · ITBIS incluido</span>
                                <asp:Label ID="lblTotalGeneral" runat="server" CssClass="total-valor" />
                            </div>
                        </div>

                    </div>
                </div>

                <a href="Historial.aspx" class="btn-volver">← Volver al historial</a>
            </asp:Panel>

            <asp:Panel ID="pnlError" runat="server" Visible="false">
                <div class="panel-error">
                    ⚠ Pedido no encontrado. El ID indicado no existe.
                </div>
            </asp:Panel>

        </main>

        <footer class="page-footer">
            © 2026 PrecisionTire · Todos los derechos reservados
        </footer>

    </form>
</body>
</html>