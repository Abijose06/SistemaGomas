using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    // Le decimos a Entity Framework cómo se llama la tabla exactamente en SQL
    [Table("tblSucursal")]
    public class Sucursal
    {
        [Key] // Le indicamos que esta es la llave primaria
        public int IdSucursal { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public bool Estado { get; set; }
    }
}