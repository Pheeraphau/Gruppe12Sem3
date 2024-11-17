namespace WebApplication1.Models
{
    public class BrukerInnmelding
    {
        public int Id { get; set; }
        public string KundeTelefon { get; set; }
        public string KundeNavn { get; set; }
        public DateTime Registreringsdato { get; set; }
        public string Kommune { get; set; }
        public string Status { get; set; }
    }
}
