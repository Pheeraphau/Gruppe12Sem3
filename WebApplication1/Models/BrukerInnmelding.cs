namespace WebApplication1.Models
{
    public class BrukerInnmelding
    {
        public int Id { get; set; }
        public string KundeNavn { get; set; }
        public DateTime Registreringsdato { get; set; }
        public string Beskrivelse { get; set; }
        public string Status { get; set; }
    }
}
