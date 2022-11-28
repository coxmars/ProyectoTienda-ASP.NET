using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_Tienda.Services
{
    public class Constants {

        public const string RolAdmin = "admin";

        // English-Spanish-French and Portuguese are supported languages
        public static readonly SelectListItem[] SupportedUICultures = new SelectListItem[] {
            new SelectListItem{Value = "es", Text = "Español"},
            new SelectListItem{Value = "en", Text = "English"}
        };

    }
}
