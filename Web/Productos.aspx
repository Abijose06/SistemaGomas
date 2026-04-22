<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Productos.aspx.cs" Inherits="WebGomas.Productos" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Catálogo de Neumáticos</title>
    <style>

        /* =====================================================
           VARIABLES Y RESET
        ===================================================== */
        :root {
            --azul:        #007BFF;
            --azul-oscuro: #0062cc;
            --azul-claro:  #e8f0fe;
            --fondo:       #F8F9FA;
            --blanco:      #ffffff;
            --gris-texto:  #6c757d;
            --gris-borde:  #e9ecef;
            --negro:       #1a1a2e;
            --sombra-card: 0 2px 12px rgba(0,0,0,0.07), 0 1px 3px rgba(0,0,0,0.05);
            --sombra-hover: 0 12px 32px rgba(0,123,255,0.13), 0 2px 8px rgba(0,0,0,0.08);
            --radio:       16px;
        }

        *, *::before, *::after {
            box-sizing: border-box;
            margin: 0;
            padding: 0;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background-color: var(--fondo);
            color: var(--negro);
            min-height: 100vh;
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


        /* =====================================================
           HERO — SECCIÓN SUPERIOR
        ===================================================== */
        .hero {
            background: linear-gradient(135deg, #0f2545 0%, #1a3a6e 60%, #1565c0 100%);
            color: var(--blanco);
            padding: 56px 40px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }

        /* Círculos decorativos en el hero */
        .hero::before,
        .hero::after {
            content: '';
            position: absolute;
            border-radius: 50%;
            opacity: 0.08;
            background: var(--blanco);
        }

        .hero::before {
            width: 400px;
            height: 400px;
            top: -120px;
            right: -80px;
        }

        .hero::after {
            width: 260px;
            height: 260px;
            bottom: -80px;
            left: -60px;
        }

        .hero-titulo {
            font-size: 36px;
            font-weight: 800;
            letter-spacing: -0.5px;
            margin-bottom: 10px;
            position: relative;
            z-index: 1;
        }

        .hero-subtitulo {
            font-size: 16px;
            opacity: 0.75;
            font-weight: 400;
            position: relative;
            z-index: 1;
        }

        /* =====================================================
           CONTENIDO PRINCIPAL
        ===================================================== */
        .contenido {
            max-width: 1200px;
            margin: 0 auto;
            padding: 48px 32px;
        }

        /* Barra superior del catálogo */
        .catalogo-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            margin-bottom: 32px;
        }

        .catalogo-titulo {
            font-size: 13px;
            font-weight: 600;
            letter-spacing: 1.5px;
            text-transform: uppercase;
            color: var(--gris-texto);
        }

        .catalogo-cantidad {
            font-size: 13px;
            color: var(--gris-texto);
            background: var(--gris-borde);
            padding: 4px 12px;
            border-radius: 20px;
        }

        /* =====================================================
           GRID DE CARDS — generado por el Repeater
        ===================================================== */
        .grid-productos {
            display: grid;
            grid-template-columns: repeat(4, 1fr);
            gap: 24px;
        }

        /* =====================================================
           CARD INDIVIDUAL
        ===================================================== */
        .card {
            background: var(--blanco);
            border-radius: var(--radio);
            box-shadow: var(--sombra-card);
            overflow: hidden;
            display: flex;
            flex-direction: column;
            transition: transform 0.25s ease, box-shadow 0.25s ease;

            /* Animación de entrada escalonada vía JS inline */
            opacity: 0;
            animation: cardEntrada 0.45s ease forwards;
        }

        @keyframes cardEntrada {
            from { opacity: 0; transform: translateY(20px); }
            to   { opacity: 1; transform: translateY(0);    }
        }

        .card:hover {
            transform: translateY(-6px);
            box-shadow: var(--sombra-hover);
        }

        /* ── Zona de imagen ── */
        .card-imagen-wrapper {
            background: linear-gradient(145deg, #f0f4ff 0%, #e6eeff 100%);
            padding: 28px;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
            height: 180px;
            overflow: hidden;
        }

        .card-imagen-wrapper::after {
            content: '';
            position: absolute;
            width: 160px;
            height: 160px;
            border-radius: 50%;
            background: rgba(0,123,255,0.05);
            top: 50%;
            left: 50%;
            transform: translate(-50%, -50%);
        }

        .card-img {
            width: 110px;
            height: 110px;
            object-fit: contain;
            position: relative;
            z-index: 1;
            transition: transform 0.35s ease;
            filter: drop-shadow(0 6px 14px rgba(0,0,0,0.12));
        }

        .card:hover .card-img {
            transform: scale(1.08) translateY(-3px);
        }

        /* ── Cuerpo de texto ── */
        .card-body {
            padding: 20px 20px 0;
            flex: 1;
            display: flex;
            flex-direction: column;
        }

        .card-marca {
            font-size: 11px;
            font-weight: 700;
            letter-spacing: 2px;
            text-transform: uppercase;
            color: var(--azul);
            margin-bottom: 6px;
        }

        .card-nombre {
            font-size: 15px;
            font-weight: 700;
            color: var(--negro);
            line-height: 1.3;
            margin-bottom: 12px;
            flex: 1;
        }

        .card-precio {
            font-size: 22px;
            font-weight: 800;
            color: var(--azul);
            letter-spacing: -0.5px;
            margin-bottom: 4px;
        }

        .card-precio-label {
            font-size: 11px;
            color: var(--gris-texto);
            margin-bottom: 16px;
        }

        /* ── Footer de la card ── */
        .card-footer {
            padding: 0 20px 20px;
        }

        .card-footer a {
            display: block;
            width: 100%;
            padding: 11px 0;
            text-align: center;
            background: var(--azul);
            color: var(--blanco);
            text-decoration: none;
            font-size: 14px;
            font-weight: 700;
            letter-spacing: 0.4px;
            border-radius: 10px;
            transition: background 0.2s ease, box-shadow 0.2s ease, transform 0.15s ease;
            box-shadow: 0 3px 10px rgba(0,123,255,0.30);
        }

        .card-footer a:hover {
            background: var(--azul-oscuro);
            box-shadow: 0 6px 18px rgba(0,123,255,0.40);
            transform: translateY(-1px);
        }

        .card-footer a:active {
            transform: translateY(0);
            box-shadow: 0 2px 6px rgba(0,123,255,0.25);
        }

        /* =====================================================
           FOOTER DE PÁGINA
        ===================================================== */
        .page-footer {
            text-align: center;
            padding: 32px;
            color: var(--gris-texto);
            font-size: 13px;
            border-top: 1px solid var(--gris-borde);
            margin-top: 16px;
        }

        /* =====================================================
           RESPONSIVE
        ===================================================== */
        @media (max-width: 1024px) {
            .grid-productos {
                grid-template-columns: repeat(3, 1fr);
            }
        }

        @media (max-width: 720px) {
            .grid-productos {
                grid-template-columns: repeat(2, 1fr);
                gap: 16px;
            }

            .hero-titulo    { font-size: 26px; }
            .contenido      { padding: 32px 16px; }
            .header         { padding: 16px 20px; }
            .header-nav     { display: none; }
        }

        @media (max-width: 420px) {
            .grid-productos {
                grid-template-columns: 1fr;
            }
        }

    </style>
</head>
<body>
    <form id="form1" runat="server">

        <!-- HEADER -->
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

        <!-- HERO -->
        <section class="hero">
            <h1 class="hero-titulo">Catálogo de Neumáticos</h1>
            <p class="hero-subtitulo">Rendimiento y seguridad en cada kilómetro</p>
        </section>

        <!-- CATÁLOGO -->
        <main class="contenido">

            <div class="catalogo-header">
                <span class="catalogo-titulo">Todos los productos</span>
                <span class="catalogo-cantidad">
                    <asp:Label ID="lblCantidad" runat="server" />
                </span>
            </div>

            <!-- GRID generado por el Repeater -->
            <div class="grid-productos">
                <asp:Repeater ID="rptProductos" runat="server">
                    <ItemTemplate>

                        <%-- Delay escalonado para la animación de entrada --%>
                        <div class="card"
                             style="animation-delay: <%# (Container.ItemIndex * 80) %>ms">

                            <!-- Imagen -->

 <div class="card-imagen-wrapper">

     <img
    src='<%# ResolveUrl(Eval("ImagenUrl").ToString()) %>'
    alt='<%# Eval("Nombre") %>'
    class="card-img" />

 </div> 

                            <!-- Cuerpo -->
                            <div class="card-body">
                                <div class="card-marca"><%# Eval("Marca") %></div>
                                <div class="card-nombre"><%# Eval("Nombre") %></div>
                                <div class="card-precio">
                                    <%# string.Format("{0:C2}", Eval("Precio")) %>
                                </div>
                                <div class="card-precio-label">Precio por unidad · ITIBIS no incluido</div>
                            </div>

                            <!-- Botón Ver detalle -->
                            <div class="card-footer">
                                <a href='<%# "DetalleProducto.aspx?id=" + Eval("Id") %>'>
                                    Ver detalle →
                                </a>
                            </div>

                        </div>

                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </main>

        <!-- FOOTER -->
        <footer class="page-footer">
            © 2026 PrecisionTire · Todos los derechos reservados
        </footer>

    </form>
</body>
</html>
