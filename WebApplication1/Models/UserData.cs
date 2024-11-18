using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserData
    {
<<<<<<< HEAD
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
        public string ConfirmPassword { get; set; }
=======
        public int Id { get; set; }
        public string? Name {  get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber {  get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
>>>>>>> 0f27bc61194e0c772f707ff1a5853a44c0c79901
    }
}
