using System.ComponentModel.DataAnnotations;

namespace Desastres.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

