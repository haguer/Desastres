using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Desastres.Web.Data.Entities;

public class  TipoDesastre
{
   
	public int Id { get; set; }

	[Required]
	[Display(Name = "Nombre de desastre")]
	[MaxLength(50, ErrorMessage = "El tipo de desastre debe tener menos de 50 caracteres.")]
	public string NombreDesastre { get; set; }
	public ICollection<Funcion> Funciones { get; set; }

}

