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

                // Save to the database
                _context.GeoChanges.Add(newGeoChange);
                _context.SaveChanges();

                // Redirect to the overview of changes
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }



        [HttpGet]
        public IActionResult RegistrationForm()
        {
            return View();
        }

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
        }

    }
}
