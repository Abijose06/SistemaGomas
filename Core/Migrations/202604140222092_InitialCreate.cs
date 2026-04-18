namespace Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.tblCliente",
                c => new
                    {
                        IdCliente = c.Int(nullable: false, identity: true),
                        IdUsuario = c.Int(nullable: false),
                        Direccion = c.String(),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdCliente);
            
            CreateTable(
                "dbo.tblEmpleado",
                c => new
                    {
                        IdEmpleado = c.Int(nullable: false, identity: true),
                        IdUsuario = c.Int(nullable: false),
                        Sueldo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FechaIngreso = c.DateTime(nullable: false),
                        Estado = c.Boolean(nullable: false),
                        IdSucursal = c.Int(),
                    })
                .PrimaryKey(t => t.IdEmpleado);
            
            CreateTable(
                "dbo.tblProducto",
                c => new
                    {
                        IdProducto = c.Int(nullable: false, identity: true),
                        Marca = c.String(nullable: false, maxLength: 100),
                        Modelo = c.String(nullable: false, maxLength: 100),
                        Medida = c.String(nullable: false, maxLength: 15),
                        PrecioVenta = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Costo = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Estado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.IdProducto);
            
            CreateTable(
                "dbo.tblUsuario",
                c => new
                    {
                        IdUsuario = c.Int(nullable: false, identity: true),
                        TipoDocumento = c.Int(nullable: false),
                        Documento = c.String(nullable: false, maxLength: 15),
                        Nombres = c.String(nullable: false, maxLength: 80),
                        Apellidos = c.String(nullable: false, maxLength: 80),
                        Telefono = c.String(nullable: false, maxLength: 20),
                        Correo = c.String(maxLength: 255),
                        Estado = c.Boolean(nullable: false),
                        ClaveHash = c.String(nullable: false, maxLength: 255),
                        Rol = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.IdUsuario);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.tblUsuario");
            DropTable("dbo.tblProducto");
            DropTable("dbo.tblEmpleado");
            DropTable("dbo.tblCliente");
        }
    }
}
