using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Desastres.Web.Data.Entities
{
    public class Usuario : IdentityUser
    {
        [Display(Name = "Nombres")]
        [MaxLength(60, ErrorMessage = "Los {0} No puedente tener más de {1} caracteres.")]
        [Required(ErrorMessage = "Los {0} es obligatorio.")]
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(60, ErrorMessage = "Los {0} No pueden tener más de {1} caracteres.")]
        [Required(ErrorMessage = "Los {0} es bligatorio.")]
        public string Apellidos { get; set; }

        [Display(Name = "Full Name")]
        public string NombreApellido => $"{Nombres} {Apellidos}";
        
    }
}
