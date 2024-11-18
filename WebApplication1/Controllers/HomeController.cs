using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        // Bruker en lokal liste i stedet for en database
        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<AreaChange> changes = new List<AreaChange>();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
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

                var newChange = new AreaChange
                {
                    GeoJson = geoJson,
                    Description = description
                };

                changes.Add(newChange); // Legger til i lokal liste
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                // Logg feilen hvis ønskelig
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

<<<<<<< HEAD
        [HttpGet]
        public ViewResult RegistrationForm()
=======


        [HttpGet]
        public IActionResult RegistrationForm()
>>>>>>> 0f27bc61194e0c772f707ff1a5853a44c0c79901
        {
            return View();
        }

<<<<<<< HEAD
        [HttpPost]
        public IActionResult RegistrationForm(UserData userData)
        {
            if (ModelState.IsValid)
            {
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
=======
        public IActionResult Overview()
        {
            return View();
        }
       

        [HttpPost]
        public IActionResult RegistrationForm(UserData userData)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Save the UserData object to the database
                    _context.UserData.Add(userData);
                    _context.SaveChanges();

                    return RedirectToAction("RegisterAreaChange");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    Console.WriteLine($"Error: {ex.Message}");
                    return View(); // You could return a view with an error message
                }
            }

            // If the model is invalid, return the same view with validation errors
            return View(userData);
>>>>>>> 0f27bc61194e0c772f707ff1a5853a44c0c79901
        }

    }
}


