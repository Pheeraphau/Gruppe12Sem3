using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Data;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly Services.GeoChangeService _geoChangeService;
        private readonly UserManager<IdentityUser> _userManager;

        private static List<PositionModel> positions = new List<PositionModel>();
        private static List<AreaChange> changes = new List<AreaChange>();

        public HomeController(
            ILogger<HomeController> logger,
            GeoChangeService geoChangeService,
            UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _geoChangeService = geoChangeService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult MineInnmeldinger()
        {
            var userId = _userManager.GetUserId(User);
            var userChanges = _geoChangeService.GetAllGeoChanges(userId);
            return View(userChanges);
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult SaksbehandlerOversikt()
        {
            var allChanges = _context.AreaChanges.ToList();
            return View(allChanges);
        }

        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RegisterAreaChange(string geoJson, string description)
        {
            try
            {
                if (string.IsNullOrEmpty(geoJson) || string.IsNullOrEmpty(description))
                {
                    return BadRequest("Invalid data.");
                }

                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                // Save to the database using Dapper
                _geoChangeService.AddGeoChange(description, geoJson, userId);

                // Redirect to the overview of changes
                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering area change.");
                throw;
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AreaChangeOverview()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var changes = _geoChangeService.GetAllGeoChanges(userId);
            return View(changes);
        }


        [HttpPost]
        [Authorize]
        public IActionResult UpdateAreaChange(int id, string description, string geoJson)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                _geoChangeService.UpdateGeoChange(id, description, geoJson, userId);

                return RedirectToAction("MineInnmeldinger");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating area change: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Saksbehandler()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public IActionResult DeleteAreaChange(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                _geoChangeService.DeleteGeoChange(id, userId);

                return RedirectToAction("MineInnmeldinger");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting area change: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation($"Edit GET action called with id={id}");

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null)
            {
                _logger.LogWarning($"GeoChange with id={id} not found for userId={userId}");
                return NotFound();
            }

            return View(geoChange);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(GeoChange model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid. Updating GeoChange aborted.");
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                // Save updated GeoChange to the database
                _geoChangeService.UpdateGeoChange(model.Id, model.Description, model.GeoJson, userId);

                return RedirectToAction("AreaChangeOverview");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating GeoChange.");
                return View("Error");
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            var geoChange = _geoChangeService.GetGeoChangeById(id, userId);
            if (geoChange == null)
            {
                return NotFound();
            }

            return View(geoChange);
        }

        [Authorize]
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            _geoChangeService.DeleteGeoChange(id, userId);

            return RedirectToAction("AreaChangeOverview");
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> UpdateOverview()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var userId = user.Id;

                var allChanges = _geoChangeService.GetAllGeoChanges(userId);
                return View(allChanges);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving GeoChanges in UpdateOverview.");
                return View("Error");
            }
        }






        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}