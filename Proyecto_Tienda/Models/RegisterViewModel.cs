using System.ComponentModel.DataAnnotations;

namespace Proyecto_Tienda.Models
{
    public class RegisterViewModel {
        [Required(ErrorMessage= "Error.Requerido")]
        [EmailAddress(ErrorMessage = "Error.Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Error.Requerido")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
