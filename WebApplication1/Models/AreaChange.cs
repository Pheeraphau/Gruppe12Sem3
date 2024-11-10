namespace WebApplication1.Models
{
    public class AreaChange
    {
        // Use int for auto-incremented primary key
        public string? Id { get; set; }
        public string? GeoJson { get; set; } // For kartdata i GeoJSON format  
        public string? Description { get; set; } // For brukerens beskrivelse  
    }
}
