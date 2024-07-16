using System.ComponentModel.DataAnnotations;

namespace CaritasBack.Models
{
    public class UsuarioLogin
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string email { get; set; }
        public string password { get; set; } 
    }
}
