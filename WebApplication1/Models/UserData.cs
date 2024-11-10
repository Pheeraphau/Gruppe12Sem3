namespace WebApplication1.Models
{
    public class UserData
    {
        public int Id { get; set; }  // Use int, and it will be auto-incremented by the database
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone {  get; set; }
            public string? Address { get; set; }
    }
}
