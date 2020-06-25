
using System;
using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Data.Entities
{
    public class Emergencia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Los {0} es obligatorio.")]
        [Display(Name = "Nombres")]
        [MaxLength(60, ErrorMessage = "Los {0} No puede tener más de {1} caracteres.")]
       
        public string Nombres { get; set; }

        [Display(Name = "Apellidos")]
        [MaxLength(60, ErrorMessage = "Los {0} No pueden tener más de {1} caracteres.")]
        [Required(ErrorMessage = "Los {0} es bligatorio.")]
        public string Apellidos { get; set; }

        [Phone]
        [Display(Name = "Teléfono")]
        [Required(ErrorMessage = "Los {0} es obligatorio.")]
        public string telefono { get; set; }
    

        [MaxLength(100, ErrorMessage = "la dirección debe ser menor de 100 caracteres.")]
        [Display(Name = "Dirección del Incide")]
        public string direccion { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora Incidente ")]

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd H:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FechaIncidente { get; set; }

        [Display(Name = "FotoIncidente")]
        public string FotoRuta { get; set; }

      
        public TipoDesastre Desastre { get; set; }

        [Display(Name = "Nombre Completo")]
        public string NombreApellido => $"{Nombres} {Apellidos}";

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Incidente")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd H:mm tt}", ApplyFormatInEditMode = true)]
        public DateTime FechaLocal => FechaIncidente.ToLocalTime();

    }
}