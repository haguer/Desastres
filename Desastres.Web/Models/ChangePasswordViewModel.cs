using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class ChangePasswordViewModel
    {
        [Display(Name = "Contraseña actual")]
        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        public string OldPassword { get; set; }

        [Display(Name = "Nueva Contraseña")]
        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmar contraseña")]
        [Required(ErrorMessage = "La Contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 20 caracteres.")]
        [Compare("NewPassword")]
        public string Confirm { get; set; }
    }
}
