using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("tblServicio")] // Nombre exacto de la tabla en tu SQL
    public class Servicio
    {
        [Key]
        public int IdServicio { get; set; }

        public string NombreServicio { get; set; }

        public decimal Precio { get; set; }

        public string Descripcion { get; set; }

        public bool Estado { get; set; } = true; // Por defecto activo
    }
}