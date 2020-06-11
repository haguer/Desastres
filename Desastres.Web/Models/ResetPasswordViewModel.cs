using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class ResetPasswordViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }
}
