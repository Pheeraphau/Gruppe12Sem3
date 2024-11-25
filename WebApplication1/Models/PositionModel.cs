using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class PositionModel
    {
        [Key] // Dette markerer egenskapen som primærnøkkel
        public int Id { get; set; }

        public string Name { get; set; } // Navn på posisjonen
        public double Latitude { get; set; } // Breddegrad for posisjonen
        public double Longitude { get; set; } // Lengdegrad for posisjonen

        // Legger til den manglende Beskrivelse-egenskapen
        public string Description { get; set; } // Beskrivelse av posisjonen
    }
}