using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class UserData
    {
        public int Id { get; set; }
        public string? Name {  get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber {  get; set; }
        public string? Address { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
}
