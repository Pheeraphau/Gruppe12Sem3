using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Authorize]
    public class GeoChangedController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<GeoChangedController> _logger;

        public GeoChangedController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            ILogger<GeoChangedController> logger)
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
        [Authorize(Roles = "User,Saksbehandler")]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
                if (geoChange == null)
                {
                    _logger.LogWarning($"GeoChange med ID {id} ble ikke funnet for sletting.");
                    return NotFound($"GeoChange med ID {id} ble ikke funnet.");
                }

                _context.GeoChanges.Remove(geoChange);
                _context.SaveChanges();

                if (_context.GeoChanges.Any(g => g.Id == id))
                {
                    _logger.LogError($"GeoChange med ID {id} ble ikke slettet.");
                    return StatusCode(500, "En feil oppstod under forsøket på å slette posten. Vennligst prøv igjen.");
                }

                _logger.LogInformation($"GeoChange med ID {id} ble slettet.");
                return RedirectToAction("MineInnmeldinger");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"En feil oppstod under sletting av GeoChange med ID {id}.");
                return StatusCode(500, "En intern serverfeil oppstod. Vennligst kontakt support.");
            }
        }

        [HttpGet]
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
                ViewData["Source"] = source;
                return View("EditInnmeldingInfo_SaksBehandler", updatedGeoChange);
            }

            var geoChange = _context.GeoChanges.FirstOrDefault(g => g.Id == id);
            if (geoChange == null)
            {
                return NotFound();
            }

            geoChange.GeoJson = updatedGeoChange.GeoJson;
            geoChange.Description = updatedGeoChange.Description;
            geoChange.Status = updatedGeoChange.Status;

            _context.SaveChanges();

            if (source == "MineInnmeldinger")
            {
                return RedirectToAction("MineInnmeldinger");
            }
            else
            {
                return RedirectToAction("SaksBehandlerOversikt");
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

            ViewData["Source"] = source;
            return View("DetailsInnmeldingSaksbehandler", geoChange);
        }
    }
}
