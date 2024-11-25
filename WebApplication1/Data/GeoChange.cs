using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace WebApplication1.Data
{
    public class GeoChange
    {
        [Key] // Primærnøkkel for EF Core
        public int Id { get; set; }

        [Required] // Påkrevd felt for GeoJSON-data
        public string GeoJson { get; set; }

        [Required] // Påkrevd felt for beskrivelse
        public string Description { get; set; }

        [BindNever] // Hindrer binding fra HTTP-forespørsler
        public string? UserId { get; set; } // Fremmednøkkel for tilknyttet bruker

        // Valgfri registreringsdato for å spore når endringen ble opprettet
        public DateTime? Registreringsdato { get; set; } = DateTime.Now;

        // Valgfritt statusfelt, standardverdi er "Innsendt"
        public string Status { get; set; } = "Innsendt";
    }
}