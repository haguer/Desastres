using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class AddEncargadoViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "El {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El {0} correo no puede tener más de {1} caracteres.")]
        [EmailAddress]
        public string Username { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(60, ErrorMessage = "Los Nombres no pueden tener más de {1} caracteres.")]
        [Required(ErrorMessage = "Los Nombres son obligatorios.")]
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "Los apellidos no pueden tener más de 60 caracteres.")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string Apellidos { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(30, ErrorMessage = "El Teléfono nio puede tener más de {1} caracteres.")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe ser minimo 8 y maximo 20 caracteres")]
        public string Password { get; set; }

        [Display(Name = "Confirmar Contraseña")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "La contraseña debe ser minimo 8 y maximo 20 caracteres")]
        [Compare("Password")]
        public string PasswordConfirm { get; set; }


    }
}
