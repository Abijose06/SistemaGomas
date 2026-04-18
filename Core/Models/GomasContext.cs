using Core.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Core.Models
{
    public class GomasContext : DbContext
    {
        public GomasContext() : base("name=GomasContext")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Esto evita que EF busque tablas con nombres en plural (como tblClientes)
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}