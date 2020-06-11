using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El Usuario es obligatorio.")]
        [EmailAddress(ErrorMessage = "El usuario es un correo electrónico.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "Las contraseñas no pueden tener menos de {1} caracteres.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}

