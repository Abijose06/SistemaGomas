<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Confirmacion.aspx.cs" Inherits="WebGomas.Confirmacion" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Confirmación de Compra — PrecisionTire</title>
    <style>

        :root {
            --azul:         #007BFF;
            --azul-oscuro:  #0062cc;
            --azul-claro:   #e8f0fe;
            --verde:        #1e8449;
            --verde-claro:  #d5f5e3;
            --verde-borde:  #a9dfbf;
            --fondo:        #F8F9FA;
            --blanco:       #ffffff;
            --gris-suave:   #f1f3f5;
            --gris-borde:   #e9ecef;
            --gris-texto:   #6c757d;
            --negro:        #1a1a2e;
            --radio-card:   20px;
            --radio-btn:    12px;
            --sombra-card:  0 4px 32px rgba(0,0,0,0.08), 0 1px 4px rgba(0,0,0,0.04);
        }

        *, *::before, *::after { box-sizing: border-box; margin: 0; padding: 0; }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: var(--fondo);
            color: var(--negro);
            min-height: 100vh;
            display: flex;
            flex-direction: column;
        }

       /* =====================================================
   HEADER
===================================================== */
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

.header-logo {
    font-size: 20px;
    font-weight: 800;
    color: var(--negro);
    letter-spacing: -0.5px;
    text-decoration: none;
    flex-shrink: 0;
}

.header-logo span { color: var(--azul); }

.header-nav {
    display: flex;
    align-items: center;
    gap: 4px;
}

.header-nav a {
    color: var(--gris-texto);
    text-decoration: none;
    font-size: 14px;
    font-weight: 500;
    padding: 6px 14px;
    border-radius: 8px;
    transition: color 0.2s, background 0.2s;
    white-space: nowrap;
}

.header-nav a:hover  { color: var(--azul); background: var(--azul-claro); }
.header-nav a.activo { color: var(--azul); font-weight: 700; background: var(--azul-claro); }

.header-user {
    display: flex;
    align-items: center;
    flex-shrink: 0;
}

.btn-login-header {
    display: inline-flex;
    align-items: center;
    gap: 6px;
    padding: 8px 16px;
    background: var(--azul);
    color: var(--blanco);
    text-decoration: none;
    font-size: 13px;
    font-weight: 600;
    border-radius: 10px;
    transition: background 0.2s, transform 0.15s;
    box-shadow: 0 2px 8px rgba(0,123,255,0.25);
}

.btn-login-header:hover {
    background: var(--azul-oscuro);
    transform: translateY(-1px);
}

.user-card {
    display: flex;
    align-items: center;
    gap: 10px;
    background: var(--gris-suave);
    border: 1px solid var(--gris-borde);
    border-radius: 12px;
    padding: 6px 6px 6px 12px;
}

.user-avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
    background: var(--azul);
    color: var(--blanco);
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 14px;
    font-weight: 700;
    flex-shrink: 0;
}

.user-datos {
    display: flex;
    flex-direction: column;
    line-height: 1.3;
}

.user-saludo {
    font-size: 10px;
    font-weight: 600;
    color: var(--gris-texto);
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.user-nombre {
    font-size: 13px;
    font-weight: 700;
    color: var(--negro);
}

.btn-logout {
    width: 32px;
    height: 32px;
    border-radius: 8px;
    background: #fff5f5;
    border: 1px solid #fecaca;
    color: #e53e3e;
    display: flex;
    align-items: center;
    justify-content: center;
    text-decoration: none;
    font-size: 16px;
    transition: background 0.2s, color 0.2s, border-color 0.2s;
}

.btn-logout:hover {
    background: #e53e3e;
    color: var(--blanco);
    border-color: #e53e3e;
}

@media (max-width: 768px) {
    .header     { padding: 0 16px; }
    .header-nav { display: none; }
    .user-datos { display: none; }
    .user-card  { padding: 6px; gap: 6px; }
}

        /* BARRA DE PROGRESO */
        .progreso-barra {
            background: var(--blanco);
            border-bottom: 1px solid var(--gris-borde);
            padding: 16px 40px;
        }

        .progreso-pasos {
            max-width: 800px;
            margin: 0 auto;
            display: flex;
            align-items: center;
            justify-content: center;
            gap: 0;
        }

        .paso {
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 13px;
            font-weight: 600;
            color: var(--gris-texto);
        }

        .paso.activo { color: var(--azul); }
        .paso.hecho  { color: var(--verde); }

        .paso-num {
            width: 26px;
            height: 26px;
            border-radius: 50%;
            border: 2px solid currentColor;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            font-weight: 700;
            flex-shrink: 0;
        }

        .paso.hecho  .paso-num { background: var(--verde); border-color: var(--verde); color: var(--blanco); }
        .paso.activo .paso-num { background: var(--azul);  border-color: var(--azul);  color: var(--blanco); }

        .paso-linea {
            flex: 1;
            height: 2px;
            background: var(--gris-borde);
            margin: 0 12px;
            max-width: 80px;
        }

        .paso-linea.hecha { background: var(--verde); }

        /* CONTENIDO */
        .pagina {
            flex: 1;
            display: flex;
            flex-direction: column;
            align-items: center;
            padding: 52px 20px 64px;
        }

        .pagina-header {
            text-align: center;
            margin-bottom: 40px;
            animation: fadeDown 0.4s ease both;
        }

        @keyframes fadeDown {
            from { opacity: 0; transform: translateY(-16px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .pagina-titulo {
            font-size: 32px;
            font-weight: 800;
            color: var(--negro);
            letter-spacing: -0.5px;
            margin-bottom: 8px;
        }

        .pagina-subtitulo {
            font-size: 15px;
            color: var(--gris-texto);
            font-weight: 400;
        }

        /* CARD PRINCIPAL */
        .card-confirmacion {
            background: var(--blanco);
            border-radius: var(--radio-card);
            box-shadow: var(--sombra-card);
            width: 100%;
            max-width: 800px;
            overflow: hidden;
            animation: fadeUp 0.5s ease 0.1s both;
        }

        @keyframes fadeUp {
            from { opacity: 0; transform: translateY(24px); }
            to   { opacity: 1; transform: translateY(0); }
        }

        .card-cabecera {
            background: linear-gradient(135deg, #0f2545 0%, #1a3a6e 60%, #1565c0 100%);
            padding: 28px 36px;
            display: flex;
            align-items: center;
            justify-content: space-between;
        }

        .card-cabecera-texto h2 { color: var(--blanco); font-size: 18px; font-weight: 700; margin-bottom: 3px; }
        .card-cabecera-texto p  { color: rgba(255,255,255,0.65); font-size: 13px; }

        .card-cabecera-icono {
            width: 48px;
            height: 48px;
            border-radius: 50%;
            background: rgba(255,255,255,0.12);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 22px;
            flex-shrink: 0;
        }

        .card-cuerpo { padding: 36px; }

        /* MENSAJE ÉXITO */
        .mensaje-exito {
            background: var(--verde-claro);
            border: 1px solid var(--verde-borde);
            border-radius: 12px;
            padding: 16px 20px;
            display: flex;
            align-items: center;
            gap: 12px;
            margin-bottom: 32px;
            animation: fadeUp 0.45s ease 0.2s both;
        }

        .exito-icono {
            width: 36px;
            height: 36px;
            border-radius: 50%;
            background: var(--verde);
            color: var(--blanco);
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            flex-shrink: 0;
        }

        .exito-texto { flex: 1; }
        .exito-texto strong { display: block; font-size: 15px; color: var(--verde); font-weight: 700; margin-bottom: 2px; }
        .exito-texto span   { font-size: 13px; color: #27ae60; }

        .label-exito { font-size: 15px; color: var(--verde); font-weight: 700; }

        /* SECCIÓN PRODUCTOS */
        .seccion-titulo {
            font-size: 12px;
            font-weight: 700;
            letter-spacing: 1.5px;
            text-transform: uppercase;
            color: var(--gris-texto);
            margin-bottom: 16px;
            padding-bottom: 12px;
            border-bottom: 1px solid var(--gris-borde);
        }

        /* GRIDVIEW */
        .grid-confirmacion { width: 100%; border-collapse: collapse; margin-bottom: 8px; }

        .grid-confirmacion thead tr th {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: 1px;
            text-transform: uppercase;
            color: var(--gris-texto);
            padding: 8px 12px;
            text-align: left;
            border-bottom: 2px solid var(--gris-borde);
        }

        .grid-confirmacion thead tr th:last-child { text-align: right; }

        .grid-confirmacion tbody tr { border-bottom: 1px solid var(--gris-suave); transition: background 0.15s; }
        .grid-confirmacion tbody tr:last-child { border-bottom: none; }
        .grid-confirmacion tbody tr:hover { background: var(--gris-suave); }

        .grid-confirmacion tbody tr td {
            padding: 16px 12px;
            font-size: 14px;
            color: var(--negro);
            vertical-align: middle;
        }

        .grid-confirmacion tbody tr td:first-child { display: flex; align-items: center; gap: 14px; }

        .miniatura-wrapper {
            width: 52px;
            height: 52px;
            border-radius: 10px;
            background: linear-gradient(145deg, #f0f4ff, #e6eeff);
            display: flex;
            align-items: center;
            justify-content: center;
            flex-shrink: 0;
            overflow: hidden;
            padding: 4px;
        }

        .miniatura-wrapper img { width: 100%; height: 100%; object-fit: contain; display: block; }

        .producto-info-nombre { font-weight: 700; font-size: 14px; color: var(--negro); margin-bottom: 3px; }
        .producto-info-detalle { font-size: 12px; color: var(--gris-texto); }

        .grid-confirmacion tbody tr td.celda-cantidad { font-weight: 600; color: var(--gris-texto); font-size: 14px; }
        .grid-confirmacion tbody tr td.celda-precio   { color: var(--gris-texto); font-size: 14px; }
        .grid-confirmacion tbody tr td.celda-subtotal { text-align: right; font-weight: 700; font-size: 15px; color: var(--negro); }

        /* TOTAL */
        .seccion-total {
            background: var(--gris-suave);
            border-radius: 14px;
            padding: 20px 24px;
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-top: 24px;
            margin-bottom: 28px;
        }

        .total-etiqueta { font-size: 14px; font-weight: 600; color: var(--gris-texto); letter-spacing: 0.3px; }
        .total-etiqueta small { display: block; font-size: 11px; font-weight: 400; color: #adb5bd; margin-top: 2px; }

        .label-total { font-size: 32px; font-weight: 800; color: var(--azul); letter-spacing: -1px; line-height: 1; }

        /* BOTONES */
        .contenedor-boton { display: flex; flex-direction: column; align-items: stretch; gap: 12px; }

        .btn-confirmar {
            width: 100%;
            padding: 18px 28px;
            font-size: 16px;
            font-weight: 700;
            letter-spacing: 0.5px;
            color: var(--blanco);
            background: var(--azul);
            border: none;
            border-radius: var(--radio-btn);
            cursor: pointer;
            transition: background 0.2s, transform 0.15s, box-shadow 0.2s;
            box-shadow: 0 4px 18px rgba(0,123,255,0.35);
        }

        .btn-confirmar:hover  { background: var(--azul-oscuro); transform: translateY(-2px); box-shadow: 0 8px 28px rgba(0,123,255,0.45); }
        .btn-confirmar:active { transform: translateY(0); box-shadow: 0 2px 8px rgba(0,123,255,0.25); }

        .btn-secundario {
            display: block;
            text-align: center;
            padding: 13px;
            font-size: 14px;
            font-weight: 600;
            color: var(--gris-texto);
            background: var(--blanco);
            border: 2px solid var(--gris-borde);
            border-radius: var(--radio-btn);
            text-decoration: none;
            transition: border-color 0.2s, color 0.2s, background 0.2s;
            cursor: pointer;
        }

        .btn-secundario:hover { border-color: var(--azul); color: var(--azul); background: var(--azul-claro); }

        .boton-nota { text-align: center; font-size: 12px; color: var(--gris-texto); margin-top: 4px; }
        .boton-nota span { color: var(--verde); font-weight: 600; }

        /* ── ACCIONES POST-CONFIRMACIÓN ── */
        .acciones-finales {
            margin-top: 8px;
            padding: 24px;
            background: var(--gris-suave);
            border-radius: 14px;
            text-align: center;
        }

        .acciones-finales-titulo {
            font-size: 14px;
            font-weight: 600;
            color: var(--gris-texto);
            margin-bottom: 16px;
            letter-spacing: 0.3px;
        }

        .acciones-finales-botones { display: flex; gap: 12px; justify-content: center; }

        .acciones-finales-botones .btn-secundario,
        .btn-ver-pedidos {
            flex: 1;
            max-width: 220px;
            padding: 13px 16px;
            text-align: center;
            font-size: 14px;
            font-weight: 600;
            border-radius: var(--radio-btn);
            text-decoration: none;
            transition: background 0.2s, transform 0.15s, box-shadow 0.2s;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
        }

        .btn-ver-pedidos {
            background: var(--azul);
            color: var(--blanco);
            border: 2px solid var(--azul);
            box-shadow: 0 4px 14px rgba(0,123,255,0.28);
        }

        .btn-ver-pedidos:hover {
            background: var(--azul-oscuro);
            border-color: var(--azul-oscuro);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0,123,255,0.38);
        }

        /* PANEL VACÍO */
        .panel-vacio {
            width: 100%;
            max-width: 800px;
            text-align: center;
            padding: 64px 32px;
            background: var(--blanco);
            border-radius: var(--radio-card);
            box-shadow: var(--sombra-card);
            animation: fadeUp 0.5s ease both;
        }

        .vacio-icono  { font-size: 56px; margin-bottom: 20px; display: block; opacity: 0.35; }
        .vacio-titulo { font-size: 20px; font-weight: 700; color: var(--negro); margin-bottom: 8px; }
        .vacio-subtitulo { font-size: 14px; color: var(--gris-texto); margin-bottom: 28px; }

        .btn-ir-catalogo {
            display: inline-block;
            padding: 13px 28px;
            background: var(--azul);
            color: var(--blanco);
            text-decoration: none;
            font-size: 15px;
            font-weight: 700;
            border-radius: var(--radio-btn);
            transition: background 0.2s, transform 0.15s;
            box-shadow: 0 4px 14px rgba(0,123,255,0.30);
        }

        .btn-ir-catalogo:hover { background: var(--azul-oscuro); transform: translateY(-2px); }

        /* FOOTER */
        .page-footer {
            text-align: center;
            padding: 28px;
            color: var(--gris-texto);
            font-size: 13px;
            border-top: 1px solid var(--gris-borde);
            margin-top: auto;
        }

        /* RESPONSIVE */
        @media (max-width: 640px) {
            .card-cuerpo    { padding: 24px 20px; }
            .card-cabecera  { padding: 22px 20px; }
            .pagina-titulo  { font-size: 24px; }
            .label-total    { font-size: 26px; }
            .progreso-barra { padding: 12px 20px; }
            .header         { padding: 16px 20px; }
            .header-nav     { display: none; }

            .grid-confirmacion tbody tr td:first-child { flex-direction: column; align-items: flex-start; gap: 8px; }

            .acciones-finales-botones { flex-direction: column; align-items: stretch; }
            .acciones-finales-botones .btn-secundario,
            .btn-ver-pedidos { max-width: 100%; }
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <header class="header">

    <!-- IZQUIERDA: Logo -->
    <a href="Productos.aspx" class="header-logo">Precision<span>Tire</span></a>

    <!-- CENTRO: Navegación -->
    <nav class="header-nav">
        <a href="Productos.aspx">Catálogo</a>
        <a href="Carrito.aspx">Carrito</a>
        <a href="Historial.aspx">Mis pedidos</a>
    </nav>

    <!-- DERECHA: Usuario -->
    <div class="header-user">

        <%-- No logueado --%>
        <asp:PlaceHolder ID="phNoLogueado" runat="server">
            <a href="Login.aspx" class="btn-login-header">
                🔐 Iniciar sesión
            </a>
        </asp:PlaceHolder>

        <%-- Logueado --%>
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

        <!-- BARRA DE PROGRESO -->
        <div class="progreso-barra">
            <div class="progreso-pasos">
                <div class="paso hecho">
                    <div class="paso-num">✓</div>
                    <span>Catálogo</span>
                </div>
                <div class="paso-linea hecha"></div>
                <div class="paso hecho">
                    <div class="paso-num">✓</div>
                    <span>Carrito</span>
                </div>
                <div class="paso-linea hecha"></div>
                <div class="paso activo">
                    <div class="paso-num">3</div>
                    <span>Confirmación</span>
                </div>
            </div>
        </div>

        <!-- CONTENIDO -->
        <main class="pagina">

            <div class="pagina-header">
                <h1 class="pagina-titulo">Confirmación de Compra</h1>
                <p class="pagina-subtitulo">Revisa los detalles finales antes de procesar tu orden</p>
            </div>

            <!-- PANEL: compra con productos -->
            <asp:Panel ID="pnlConfirmacion" runat="server" Visible="false"
                       CssClass="card-confirmacion">

                <div class="card-cabecera">
                    <div class="card-cabecera-texto">
                        <h2>Resumen de tu orden</h2>
                        <p>Verifica cada producto antes de confirmar</p>
                    </div>
                    <div class="card-cabecera-icono">🛒</div>
                </div>

                <div class="card-cuerpo">

                    <!-- Mensaje de éxito -->
                    <asp:Panel ID="pnlMensajeExito" runat="server" Visible="false">
                        <div class="mensaje-exito">
                            <div class="exito-icono">✓</div>
                            <div class="exito-texto">
                                <asp:Label
                                    ID="lblMensaje"
                                    runat="server"
                                    CssClass="label-exito" />
                                <span>Tu pedido será procesado en breve</span>
                            </div>
                        </div>
                    </asp:Panel>
                    <%-- ↑ cierra pnlMensajeExito --%>

                    <!-- Título sección -->
                    <div class="seccion-titulo">Productos en tu orden</div>

                    <!-- GridView -->
                    <asp:GridView
                        ID="gvConfirmacion"
                        runat="server"
                        AutoGenerateColumns="false"
                        CssClass="grid-confirmacion"
                        GridLines="None">

                        <HeaderStyle CssClass="grid-header" />

                        <Columns>

                            <asp:TemplateField HeaderText="Producto">
                                <ItemTemplate>
                                    <div class="miniatura-wrapper">
                                        <img
                                            src='<%# ResolveUrl((Eval("ImagenUrl") ?? "~/images/GomaPilotSport4.png").ToString()) %>'
                                            alt='<%# Eval("Nombre") %>'
                                            style="width:100%; height:100%; object-fit:contain;" />
                                    </div>
                                    <div class="producto-info-nombre">
                                        <%# Eval("Nombre") %>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField
                                DataField="Cantidad"
                                HeaderText="Cant."
                                ItemStyle-CssClass="celda-cantidad" />

                            <asp:BoundField
                                DataField="Precio"
                                HeaderText="Precio unit."
                                DataFormatString="{0:C2}"
                                ItemStyle-CssClass="celda-precio" />

                            <asp:BoundField
                                DataField="Subtotal"
                                HeaderText="Subtotal"
                                DataFormatString="{0:C2}"
                                ItemStyle-CssClass="celda-subtotal" />

                        </Columns>

                    </asp:GridView>

                    <!-- Total con desglose ITBIS -->
<div class="seccion-total" style="flex-direction:column; align-items:stretch; gap:10px;">
    <div style="display:flex; justify-content:space-between; font-size:14px; color:#6c757d; padding:4px 0;">
        <span>Subtotal sin ITBIS</span>
        <asp:Label ID="lblSubtotal" runat="server" />
    </div>
    <div style="display:flex; justify-content:space-between; font-size:14px; color:#6c757d; padding:4px 0; border-bottom:1px solid #e9ecef; padding-bottom:12px;">
        <span>ITBIS (18%)</span>
        <asp:Label ID="lblItbis" runat="server" />
    </div>
    <div style="display:flex; justify-content:space-between; align-items:center; padding-top:4px;">
        <div class="total-etiqueta">
            Total a pagar
            <small>ITBIS incluido · Envío gratis</small>
        </div>
        <asp:Label
            ID="lblTotal"
            runat="server"
            CssClass="label-total" />
    </div>
</div> 

                    <!-- Botones principales -->
                    <div class="contenedor-boton">
                        <asp:Button
                            ID="btnConfirmar"
                            runat="server"
                            Text="✓  Confirmar compra"
                            CssClass="btn-confirmar"
                            OnClick="btnConfirmar_Click" />

                        <asp:Button
                            ID="btnVolverCarrito"
                            runat="server"
                            Text="← Volver al carrito"
                            CssClass="btn-secundario"
                            OnClick="btnVolverCarrito_Click"
                            CausesValidation="false" />

                        <p class="boton-nota">
                            <span>🔒 Pago seguro</span> · Tu información está protegida
                        </p>
                    </div>

                    <!-- Acciones post-confirmación -->
                    <asp:Panel ID="pnlAccionesFinales" runat="server" Visible="false">
                        <div class="acciones-finales">
                            <div class="acciones-finales-titulo">
                                ¿Qué deseas hacer ahora?
                            </div>
                            <div class="acciones-finales-botones">
                                <a href="Productos.aspx" class="btn-secundario">
                                    🏷️ Seguir comprando
                                </a>
                                <a href="Historial.aspx" class="btn-ver-pedidos">
                                    📋 Ver mis pedidos
                                </a>
                            </div>
                        </div>
                    </asp:Panel>
                    <%-- ↑ cierra pnlAccionesFinales --%>

                </div>
                <%-- ↑ cierra card-cuerpo --%>

            </asp:Panel>
            <%-- ↑ cierra pnlConfirmacion --%>

            <!-- PANEL: carrito vacío -->
            <asp:Panel ID="pnlVacio" runat="server" Visible="false"
                       CssClass="panel-vacio">
                <span class="vacio-icono">🛒</span>
                <p class="vacio-titulo">No hay productos en el carrito</p>
                <p class="vacio-subtitulo">
                    Agrega productos desde el catálogo para continuar.
                </p>
                <a href="Productos.aspx" class="btn-ir-catalogo">
                    Ver catálogo
                </a>
            </asp:Panel>
            <%-- ↑ cierra pnlVacio --%>

        </main>

        <!-- FOOTER -->
        <footer class="page-footer">
            © 2026 PrecisionTire · Todos los derechos reservados
        </footer>

    </form>
</body>
</html>