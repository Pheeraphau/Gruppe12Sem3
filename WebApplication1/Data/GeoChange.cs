using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Data
{
    public class GeoChange
    {
        [Key] // EF Core primary key
        public int Id { get; set; }

        [Required] // Required field for GeoJSON data
        public string GeoJson { get; set; }

        [Required] // Required field for description
        public string Description { get; set; }

        [BindNever] // Prevent binding from HTTP requests
        public string? UserId { get; set; } // Foreign Key for the associated user

        // Optional registration date for tracking when the change was created
        public DateTime? Registreringsdato { get; set; } = DateTime.Now;

        // Optional status field, defaulting to "Innsendt"
        public string Status { get; set; } = "Innsendt";

        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
    }
}
