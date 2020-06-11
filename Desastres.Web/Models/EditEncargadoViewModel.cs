using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class EditEncargadoViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Nombres")]
        [MaxLength(60, ErrorMessage = "Los Nombres no pueden tener más de {1} caracteres.")]
        [Required(ErrorMessage = "Los Nombres son obligatorios.")]
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(50, ErrorMessage = "Los apellidos no pueden tener más de 60 caracteres.")]
        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        public string Apellidos { get; set; }

        [Display(Name = "Teléfono")]
        [MaxLength(30, ErrorMessage = "El Teléfono no puede tener más de {1} caracteres.")]
        public string PhoneNumber { get; set; }
    }
}
