using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Data.Entities
{
    public  class Entidad 
    {
        
        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es obligatorio.")]
        [Display(Name = "Nombre Entidad")]
        [MaxLength(50, ErrorMessage = "El nombre de la entidad debe ser menor de 50 caractreres.")]        
        public string NombreEntidad { get; set; }
        [Required(ErrorMessage = "El {0} es obligatorio.")]
        [Phone]
        [Display(Name = "Teléfono")]
        public string telefono{ get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "El {0} es obligatorio.")]
        [Display(Name = "Correo Electrónico")]
        public string email { get; set; }
        [MaxLength(100, ErrorMessage = "la direcciñon debe ser menor de 100 caracteres.")]
        [Display(Name = "Dirección")]
        public string direccion { get; set; }
        public ICollection<Funcion> Funciones { get; set; }       
    }
}
