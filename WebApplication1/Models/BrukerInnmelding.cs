using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BrukerInnmelding
    {   // La til [Required] og [StringLength] for å validere input
        public int Id { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Navnet kan ikke ha mer enn 50 tegn")]
        public string KundeNavn { get; set; }
        public DateTime Registreringsdato { get; set; }
        [Required]
        [StringLength(500, ErrorMessage = "Beskrivelsen kan ikke ha mer enn 500 tegn")]
        public string Beskrivelse { get; set; }
        public string Status { get; set; }
    }
}
