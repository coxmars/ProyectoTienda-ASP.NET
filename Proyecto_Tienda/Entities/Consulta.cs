using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Proyecto_Tienda.Entities
{
    public class Consulta {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Nombre { get; set; }
        [StringLength(100)]
        [Required]
        public string Descripcion { get; set; }
        [Required]
        [EmailAddress]
        public string Correo { get; set; }
        [Required]
        [Phone]
        public string Telefono { get; set; }
        [Required]
        public DateTime FechaCreacion { get; set; }
    }
}
