using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Tienda.Entities
{
    public class Producto {

        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }
        [StringLength(100)]
        [Required]
        public string Descripcion { get; set; }
        [Required]
        public double Precio { get; set; }
        [Required]
        public int Cantidad { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }

    }
}
