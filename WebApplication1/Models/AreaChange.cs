using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class AreaChange
    {
        // Marker som primærnøkkel
        public int Id { get; set; } // Bruk int for en auto-incremented primærnøkkel

        [Required] // Marker som påkrevd felt
        public string? GeoJson { get; set; } // Kartdata i GeoJSON-format

        [Required] // Marker som påkrevd felt
        [MaxLength(500)] // Begrens lengden på beskrivelsen
        public string? Description { get; set; } // Brukerens beskrivelse

        [Required]
        public DateTime SubmittedAt { get; set; } = DateTime.Now; // Legg til et tidsstempel for innsending
    }
}
