using System;
using System.ComponentModel.DataAnnotations;
using Desastres.Web.Data.Entities;

namespace Desastres.Web.Models
{
    public class DataViewModel
    {


        [Display(Name = "Dirección Incidente")]
        public string direccion { get; set; }

     
        public int DesastreId { get; set; }

        [Display(Name = "Nombre y Apellido")]
        public string NombreApellido { get; set; }

        [Display(Name = "Fecha Incidente")]
        public DateTime FechaLocal { get; set; }

        [Display(Name = "Tipo Desastre")]
        public string NombreDesastres { get; set; }

        public int EmergenciaId { get; set; }
    }
}
