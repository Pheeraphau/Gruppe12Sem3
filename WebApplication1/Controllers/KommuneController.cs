using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;

public class KommuneController : Controller
{
    private readonly KommuneService _kommuneService;

    public KommuneController(KommuneService kommuneService)
    {
        _kommuneService = kommuneService;
    }

    public IActionResult KommuneSearch()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Search(string kommunenummer)
    {
        if (string.IsNullOrEmpty(kommunenummer))
        {
            ViewData["ErrorMessage"] = "Vennligst oppgi et kommunenummer.";
            return View("KommuneSearch");
        }

        // Call the service with the kommunenummer
        var kommuneInfo = await _kommuneService.GetKommuneInfoAsync(kommunenummer);
        if (kommuneInfo == null)
        {
            ViewData["ErrorMessage"] = "Dette kommunenummeret eksisterer ikke.";
            return View("KommuneSearch"); // Return back to the search view with the error
        }

        return View("KommuneResults", kommuneInfo); // Pass the data to the results view
    }
}
