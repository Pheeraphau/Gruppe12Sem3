using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class RegisterViewModel
    {   // La til [Required] og [StringLength] for å validere input
        [Required]
        [StringLength(50, ErrorMessage = "Brukernavnet kan ikke ha mer enn 50 tegn")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Passordet må være minst 6 tegn langt.")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passordet og konfirmasjonen er ikke like")]
        public string ConfirmPassword { get; set; }
    }
}
