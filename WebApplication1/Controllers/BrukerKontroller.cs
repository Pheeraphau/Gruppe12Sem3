using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class BrukerKontroller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
