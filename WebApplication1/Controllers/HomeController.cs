using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        //EF
        private readonly ApplicationDbContext _context;

        private static List<PositionModel> positions = new List<PositionModel>();

        private static List<AreaChange> changes = new List<AreaChange>();

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult MineInnmeldinger()
        {
            // Her henter vi data fra databasen. Bruk _context hvis du har en databasekobling.
            var innmeldinger = new List<Innmelding>
{
    new Innmelding { Id = 1, Registreringsdato = new DateTime(2024, 12, 21), Forklaring = "Invitasjon", Status = "Fullført" },
    new Innmelding { Id = 2, Registreringsdato = new DateTime(2024, 10, 02), Forklaring = "Invitasjon", Status = "Mottat" },
    new Innmelding { Id = 3, Registreringsdato = new DateTime(2024, 06, 14), Forklaring = "Invitasjon", Status = "Under behandling" }
};

            return View(innmeldinger);
        }

        public IActionResult SaksbehandlerOversikt()
        {
            // Simulerte data for eksempelbruk; i en reell applikasjon bør disse dataene hentes fra en database.
            var innmeldinger = new List<BrukerInnmelding>
    {
        new BrukerInnmelding { Id = 103, KundeTelefon = "902 57 611", KundeNavn = "Bjørn", Registreringsdato = new DateTime(2024, 12, 21), Kommune = "Oslo", Status = "Fullført" },
        new BrukerInnmelding { Id = 84, KundeTelefon = "454 17 463", KundeNavn = "Christian", Registreringsdato = new DateTime(2024, 10, 02), Kommune = "Agder", Status = "Mottatt" },
        new BrukerInnmelding { Id = 23, KundeTelefon = "405 12 424", KundeNavn = "Maria", Registreringsdato = new DateTime(2024, 06, 14), Kommune = "Nordre Follo", Status = "Under behandling" }
    };

            return View(innmeldinger);
        }


        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Display overview of area changes using a local list
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            return View(changes); // Viser lokale data
        }

        [HttpPost]
        public IActionResult CorrectMap(PositionModel model)
        {
            if (ModelState.IsValid)
            {
                positions.Add(model);
                return View("CorrectionOverview", positions);
            }

            return View();
        }

        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                // Valider inputdata
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("GeoJSON and Description cannot be empty.");
                }

                // Opprett en ny AreaChange basert på innsendte data
                var newChange = new AreaChange
                {
                    GeoJson = geoJson,
                    Description = description,
                    SubmittedAt = DateTime.Now // Registrer tidspunkt for innsending (valgfritt)
                };

                // Lagre i databasen
                _context.AreaChanges.Add(newChange);
                _context.SaveChanges(); // Utfør databasen-lagring

                // Returner til oversiktssiden etter lagring
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                // Logg feilen hvis nødvendig (valgfritt)
                Console.WriteLine($"An error occurred: {ex.Message}");

                // Returner en statuskode 500 ved feil
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [HttpGet]
        public ViewResult RegistrationForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RegistrationForm(Models.UserData userData)
        {
            if (ModelState.IsValid)
            {
                _context.Users.Add(userData);
                _context.SaveChanges(); // Lagre data i databasen

                return RedirectToAction("RegisterAreaChange");
            }

            return View(userData);
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            bool isUserValid = ValidateUser(email, password);

            if (isUserValid)
            {
                return RedirectToAction("RegisterAreaChange");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        private bool ValidateUser(string email, string password)
        {
            return true; // Midlertidig tillater alle brukere
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}
