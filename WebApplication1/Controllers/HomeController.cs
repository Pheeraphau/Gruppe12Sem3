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

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
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

        [HttpGet]
        public IActionResult CorrectMap()
        {
            return View();
        }

        // Display overview of area changes
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            var changes_db = _context.GeoChanges.ToList();
            return View(changes_db);
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

        // Handle form submission to register area change
        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data");
                }

                var newGeoChange = new GeoChange
                {
                    GeoJson = geoJson,
                    Description = description
                };

            changes.Add(newChange);
            return RedirectToAction("AreaChangeOverview");
        }

        // GET for registreringsskjema
        [HttpGet]
        public ViewResult RegistrationForm()
        {
            return View();
        }

        // POST for registreringsskjema med omdirigering til RegisterAreaChange etter registrering
        [HttpPost]
        public IActionResult RegistrationForm(UserData userData)
        {
            if (ModelState.IsValid)
            {
                // Legg til brukeren til en database eller en liste hvis ønsket (valgfritt)

                // Omdiriger til RegisterAreaChange etter registrering
                return RedirectToAction("RegisterAreaChange");
            }

            // Hvis modellen ikke er gyldig, vis skjemaet på nytt
            return View(userData);
        }

        // Login-handling for omdirigering til RegisterAreaChange
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            bool isUserValid = ValidateUser(email, password);

            if (isUserValid)
            {
                // Omdiriger til RegisterAreaChange etter vellykket innlogging
                return RedirectToAction("RegisterAreaChange");
            }

            // Hvis innlogging feiler, vis logg inn-siden på nytt med feilmelding
            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        // Metode for å validere brukerinformasjon (eksempel)
        private bool ValidateUser(string email, string password)
        {
            // Legg til valideringslogikk her
            return true; // Sett til true for å tillate alle brukere midlertidig
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


