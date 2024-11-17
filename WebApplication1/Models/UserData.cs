using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserData
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        public string? Username { get; set; }
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
        public string? ConfirmPassword { get; set; }
    }
}

