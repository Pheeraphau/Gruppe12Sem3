namespace WebApplication1.Models
{
    public class Innmelding
    {
        public int Id { get; set; }
        public DateTime Registreringsdato { get; set; }
        public string Beskrivelse { get; set; }
        public string Status { get; set; }
        public string Kundenavn { get; set; }
        public string KundeTelefon { get; set; }
    }
}
