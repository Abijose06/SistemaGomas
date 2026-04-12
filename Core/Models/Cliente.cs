using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("tblCliente")] // Esto le dice a C# que la tabla se llama así en SQL
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string Direccion { get; set; }
        public bool Estado { get; set; }
    }
}