using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class GeoChangeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GeoChangeController> _logger;

        public GeoChangeController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<GeoChangeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public IActionResult RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Ugyldig data");
                }

                var userId = _userManager.GetUserId(User);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("Bruker er ikke logget inn");
                }

                var newGeoChange = new GeoChange
                {
                    GeoJson = geoJson,
                    Description = description,
                    UserId = userId,
                    Registreringsdato = DateTime.Now,
                    Status = "Innsendt"
                };

                _context.GeoChanges.Add(newGeoChange);
                _context.SaveChanges();

                var userChanges = _context.GeoChanges
                    .Where(g => g.UserId == userId)
                    .ToList();

                return View("AreaChangeOverview", userChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}");
                throw;
            }
        }

        [HttpGet]
        [Authorize(Roles = "User")]
        public IActionResult AreaChangeOverview()
        {
            var userId = _userManager.GetUserId(User);
            var userChanges = _context.GeoChanges
                .Where(g => g.UserId == userId)
                .ToList();

            return View(userChanges);
        }

        [HttpGet]
        [Authorize(Roles = "Saksbehandler")]
        public IActionResult SaksBehandlerOversikt(string searchTerm)
        {
            ViewData["SearchTerm"] = searchTerm;

            var data = _context.GeoChanges
                .Where(g =>
                    string.IsNullOrEmpty(searchTerm) ||
                    g.Id.ToString().Contains(searchTerm) ||
                    _context.Users.Where(u => u.Id == g.UserId)
                                  .Select(u => u.UserName)
                                  .FirstOrDefault()
                                  .Contains(searchTerm) ||
                    g.Description.Contains(searchTerm))
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id, string source = null)
        {
            try
            {
                var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
                if (geoChange == null)
                {
                    _logger.LogWarning($"GeoChange with ID {id} was not found for deletion.");
                    return NotFound($"GeoChange with ID {id} was not found.");
                }

                // Ensure that users can only delete their own messages
                var userId = _userManager.GetUserId(User);
                if (geoChange.UserId != userId && !User.IsInRole("Saksbehandler"))
                {
                    return Forbid();
                }

                _context.GeoChanges.Remove(geoChange);
                _context.SaveChanges();

                _logger.LogInformation($"GeoChange with ID {id} was deleted.");
                if (source == "MineInnmeldinger")
                {
                    return RedirectToAction("MineInnmeldinger", "GeoChange");
                }
                else
                {
                    return RedirectToAction("SaksBehandlerOversikt");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting GeoChange with ID {id}.");
                return StatusCode(500, "An internal server error occurred. Please contact support.");
            }
        }



        [HttpGet]
        [Authorize(Roles = "User, Saksbehandler")]
        public async Task<IActionResult> Edit(int id)
        {
            var geoChange = await _context.GeoChanges.FindAsync(id);
            if (geoChange == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var isSaksbehandler = roles.Contains("Saksbehandler");
            var isUser = roles.Contains("User");

            ViewBag.CanEditStatus = isSaksbehandler; // Saksbehandler can edit Status
            ViewBag.CanEditDescription = isUser; // User can edit Description

            return View(geoChange);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(int id, GeoChange updatedGeoChange)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedGeoChange);
            }

            var geoChange = await _context.GeoChanges.FindAsync(id);
            if (geoChange == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            var isSaksbehandler = roles.Contains("Saksbehandler");
            var isUser = roles.Contains("User");

            if (isUser)
            {
                // User can only edit the GeoJson and Description, but not Status
                geoChange.GeoJson = updatedGeoChange.GeoJson;
                geoChange.Description = updatedGeoChange.Description;
            }
            else if (isSaksbehandler)
            {
                // Saksbehandler can edit everything, including Status
                geoChange.GeoJson = updatedGeoChange.GeoJson;
                geoChange.Description = updatedGeoChange.Description;
                geoChange.Status = updatedGeoChange.Status;
            }
            else
            {
                return Forbid();
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("MineInnmeldinger", "GeoChange");
        }

        // GET: Edit GeoChange information for Saksbehandler
        [HttpGet]
        [Authorize(Roles = "Saksbehandler")]
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

            [HttpGet]
        public IActionResult DetailsInnmeldingSaksbehandler(int id, string source = null)
        {
            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound();
            }

            ViewData["Source"] = source;
            return View("DetailsInnmeldingSaksbehandler", geoChange);
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

    }
}
