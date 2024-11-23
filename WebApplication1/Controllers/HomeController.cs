using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Linq;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // For database operations
        private readonly UserManager<IdentityUser> _userManager; // For user operations

        // Constructor with Dependency Injection
        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        private static List<PositionModel> positions = new List<PositionModel>();

        
        [HttpGet]
        [Authorize]
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

        // Display overview of area changes
        [HttpGet]
        [Authorize(Roles = "Bruker")] // Ensure only logged-in users can access
        public IActionResult AreaChangeOverview()
        {
            // Get the logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Fetch only area changes that belong to this user
            var userChanges = _context.GeoChanges
                .Where(g => g.UserId == userId) // Filter by user ID
                .ToList();

            // Pass the filtered changes to the view
            return View(userChanges);
        }

        // Handle form submission to register area change
        [HttpPost]
        [Authorize(Roles = "User")] // Ensure only logged-in users can access
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Ugyldig data");
                }

                // Get the currently logged-in user's ID
                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Bruker er ikke logget inn");
                }

                // Create a new GeoChange object
                var newGeoChange = new GeoChange
                {
                    GeoJson = geoJson,
                    Description = description,
                    UserId = userId, // Associate the change with the logged-in user
                    Registreringsdato = DateTime.Now, // Record the registration date
                    Status = "Innsendt"
                };

                // Save the new GeoChange to the database
                _context.GeoChanges.Add(newGeoChange);
                _context.SaveChanges();

                // Fetch all area changes for the logged-in user
                var userChanges = _context.GeoChanges
                    .Where(g => g.UserId == userId) // Filter by user ID
                    .ToList();

                // Return the updated overview view with the user's changes
                return View("AreaChangeOverview", userChanges);
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
        [Authorize(Roles = "User")]
        public IActionResult MineInnmeldinger()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not logged in"); // Ensure user is authenticated
            }

            var innmeldinger = _context.GeoChanges
                .Where(g => g.UserId == userId) // Only fetch messages for the logged-in user
                .Select(g => new Innmelding
                {
                    Id = g.Id,
                    Registreringsdato = g.Registreringsdato ?? DateTime.Now,
                    Beskrivelse = g.Description ?? "No description available",
                    Status = g.Status
                })
                .ToList();

            return View(innmeldinger); // Return the user's messages
        }






        [HttpGet]
        [Authorize(Roles = "Saksbehandler")]
        public IActionResult SaksBehandlerOversikt(string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            // Fetch and filter data
            var data = _context.GeoChanges
                .Where(g =>
                    string.IsNullOrEmpty(searchTerm) ||
                    g.Id.ToString().Contains(searchTerm) ||
                    _context.Users.Where(u => u.Id == g.UserId)
                                  .Select(u => u.UserName)
                                  .FirstOrDefault()
                                  .Contains(searchTerm) ||
                    g.Description.Contains(searchTerm)
                )
                .Select(g => new BrukerInnmelding
                {
                    Id = g.Id,
                    KundeNavn = _context.Users
                        .Where(u => u.Id == g.UserId)
                        .Select(u => u.UserName)
                        .FirstOrDefault() ?? "N/A",
                    Registreringsdato = g.Registreringsdato ?? DateTime.Now,
                    Beskrivelse = g.Description ?? "No description available",
                    Status = g.Status
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


        public IActionResult Edit(int id)
        {
            var geoChange = _context.GeoChanges.Find(id);
            if (geoChange == null)
            {
                return NotFound();
            }

            var statusOptions = new List<SelectListItem>
    {
        new SelectListItem { Value = "Innsendt", Text = "Innsendt" },
        new SelectListItem { Value = "Under behandling", Text = "Under behandling" },
        new SelectListItem { Value = "Godkjent", Text = "Godkjent" }
    };

            ViewBag.StatusOptions = new SelectList(statusOptions, "Value", "Text", geoChange.Status);

            return View(geoChange);
        }


        [HttpPost]
        public IActionResult Edit(int id, GeoChange updatedGeoChange, string source = null)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Source"] = source; // Pass the source back to the view in case of errors
                return View("EditInnmeldingInfo_SaksBehandler", updatedGeoChange);
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

            // Redirect based on the source
            if (source == "MineInnmeldinger")
            {
                return RedirectToAction("MineInnmeldinger"); // Redirect to "Mine Innmeldinger"
            }
            else
            {
                return RedirectToAction("SaksBehandlerOversikt"); // Redirect to "Saks Behandler Oversikt"
            }
        }



        [HttpGet]
        public IActionResult Details(int id, string source = null)
        {
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound();
            }

            ViewData["Source"] = source; // Pass the source to the view
            return View("DetailsInnmeldingSaksbehandler", geoChange);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOut()
        {
            // Clear the authentication cookies
            await HttpContext.SignOutAsync();

            // Redirect to the homepage
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddGeoChange(string geoJson, string description)
        {
            if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
            {
                return BadRequest("Invalid data");
            }

            // Get the currently logged-in user's ID
            var userId = _userManager.GetUserId(User);

            // Create a new GeoChange object
            var geoChange = new GeoChange
            {
                GeoJson = geoJson,
                Description = description,
                UserId = userId,
                Registreringsdato = DateTime.Now // Set the current date
            };

            // Save the new GeoChange to the database
            _context.GeoChanges.Add(geoChange);
            _context.SaveChanges();

            return RedirectToAction("MineInnmeldinger");
        }



        [HttpGet]
        public IActionResult DetailsInnmeldingSaksbehandler(int id, string source = null)
        {
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound(); // Return 404 if record doesn't exist
            }

            ViewData["Source"] = source; // Pass the source page info to the view
            return View(geoChange); // Render the details view
        }


    }

}
