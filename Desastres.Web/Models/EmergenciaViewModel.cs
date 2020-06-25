
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Desastres.Web.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Desastres.Web.Models
{
    public class EmergenciaViewModel : Emergencia
    {
        [Required(ErrorMessage = "Debe seleccionar un tipo de desastre.")]
        [Display(Name = "Tipo Desastre")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un Desastre.")]
        public int DesastresId { get; set; }
        public string fecha {get; set;}
        public string hora { get; set; }
        
        [Display(Name = "Foto del Incidente (Opcional)")]
        public IFormFile FotoDesastre { get; set; }

        public IEnumerable<SelectListItem> Desastres { get; set; }
    }
}


