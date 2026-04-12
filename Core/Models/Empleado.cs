using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models
{
    [Table("tblEmpleado")]
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }
        public int IdUsuario { get; set; }
        public decimal Sueldo { get; set; }
        public DateTime FechaIngreso { get; set; }
        public bool Estado { get; set; }
        public int? IdSucursal { get; set; }
    }
}