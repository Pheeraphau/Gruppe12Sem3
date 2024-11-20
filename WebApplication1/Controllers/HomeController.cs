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

        [HttpGet]
        public IActionResult MineInnmeldinger()
        {
            // Fetch data from the GeoChanges database
            var innmeldinger = _context.GeoChanges
                .Select(g => new Innmelding
                {
                    Id = g.Id,
                    Registreringsdato = DateTime.Now, // Replace with actual date if available in GeoChanges
                    Beskrivelse = g.Description ?? "No description available",
                    Status = g.Status // Replace with actual status if available
                })
                .ToList();

            return View(innmeldinger);
        }

        [HttpGet]
        public IActionResult SaksBehandlerOversikt(string searchTerm)
        {
            // Pass the search term back to the view
            ViewData["SearchTerm"] = searchTerm;

            // Fetch and filter data from GeoChanges DbSet
            var data = _context.GeoChanges
                .Where(g => string.IsNullOrEmpty(searchTerm) ||
                            (g.Description != null && g.Description.Contains(searchTerm)))
                .Select(g => new BrukerInnmelding
                {
                    Id = g.Id,
                    KundeTelefon = "N/A", // Placeholder as GeoChange doesn't have this field
                    KundeNavn = "N/A",    // Placeholder
                    Registreringsdato = DateTime.Now, // Placeholder, since GeoChange doesn't have a date
                    Beskrivelse = g.Description ?? "No description available", // Using Description as Kommune
                    Status = g.Status // Placeholder status
                })
                .ToList();

            return View(data);
        }

        // Updated Delete method
        [HttpPost]
        [HttpGet]
        [HttpPost]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            try
            {
                // Try to find the record for Innmelding
                var innmelding = _context.GeoChanges.FirstOrDefault(i => i.Id == id);

                // Try to find the record for BrukerInnmelding
                var brukerInnmelding = _context.GeoChanges.FirstOrDefault(b => b.Id == id);

                // Try to find the record for GeoChange
                var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);

                if (innmelding == null && brukerInnmelding == null && geoChange == null)
                {
                    return NotFound(); // Return 404 if no record is found
                }

                // Remove the appropriate record based on what was found
                if (innmelding != null)
                {
                    _context.GeoChanges.Remove(innmelding);
                }
                else if (brukerInnmelding != null)
                {
                    _context.GeoChanges.Remove(brukerInnmelding);
                }
                else if (geoChange != null)
                {
                    _context.GeoChanges.Remove(geoChange);
                }

                // Save changes to the database
                _context.SaveChanges();

                // Get the source parameter from the query string
                string source = Request.Query["source"];

                // Redirect based on the source
                if (source == "MineInnmeldinger")
                {
                    return RedirectToAction("MineInnmeldinger"); // Redirect back to "MineInnmeldinger" page
                }
                else
                {
                    return RedirectToAction("SaksBehandlerOversikt"); // Redirect back to "SaksBehandlerOversikt" page
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                // Handle the exception (you could log it or display an error page)
                return StatusCode(500, "An error occurred while trying to delete the record.");
            }
        }


        [HttpGet]
        public IActionResult Edit(int id, string source = null)
        {
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound();
            }

            ViewData["Source"] = source; // Pass the source page info to the view
            return View(geoChange);
        }

        [HttpPost]
        public IActionResult Edit(int id, GeoChange updatedGeoChange)
        {
            if (!ModelState.IsValid)
            {
                return View("EditInnmeldingInfo_SaksBehandler", updatedGeoChange); // Explicitly specify the view name
            }

            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound(); // Return 404 if record doesn't exist
            }

            // Update the record
            geoChange.GeoJson = updatedGeoChange.GeoJson;
            geoChange.Description = updatedGeoChange.Description;
            geoChange.Status = updatedGeoChange.Status;

            _context.SaveChanges(); // Commit changes to the database

            return RedirectToAction("SaksBehandlerOversikt"); // Redirect back to the overview
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound(); // Return 404 if record doesn't exist
            }

            return View("DetailsInnmeldingSaksbehandler", geoChange); // Explicitly specify the view name
        }

        public IActionResult EditInnmeldingInfo_SaksBehandler(int id, string source)
        {
            // Retrieve the GeoChange model from the database using the id
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);

            if (geoChange == null)
            {
                return NotFound();
            }

            // Pass the model and source to the view
            ViewData["Source"] = source;
            return View(geoChange); // Ensure this returns the correct view name
        }

        public IActionResult DetailsInnmeldingSaksbehandler(int id, string source)
        {
            // Retrieve the model (GeoChange or relevant model from the database)
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);

            if (geoChange == null)
            {
                return NotFound();
            }

            // Pass the source (from MineInnmeldinger) to the view for context
            ViewData["Source"] = source;

    // Return the Details view
    return View(geoChange);
}


    }

}
