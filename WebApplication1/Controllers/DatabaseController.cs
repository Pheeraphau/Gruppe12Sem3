using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;

namespace WebApplication1.Controllers
{
    public class DatabaseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DatabaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Get the name of the currently connected database
            var databaseName = _context.Database.GetDbConnection().Database;

            // Return the name of the database as a response
            return Content($"Connected to database: {databaseName}");
        }
    }
}
