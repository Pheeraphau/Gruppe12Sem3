using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Brukernavn")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "E-postadresse")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene samsvarer ikke ")]
        [Display(Name = "Bekreft passord")]
        public string ConfirmPassword { get; set; }
    }
}
