namespace WebApplication1.Models
{
    public class Innmelding
    {
        public int Id { get; set; }
        public DateTime Registreringsdato { get; set; }
        public string Forklaring { get; set; }
        public string Status { get; set; }
    }
}
