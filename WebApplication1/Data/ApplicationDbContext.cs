using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSet for GeoChange
        public DbSet<GeoChange> GeoChanges { get; set; }

        public DbSet<Models.UserData> Users { get; set; } // For brukerregistrering
        public DbSet<AreaChange> AreaChanges { get; set; } // For registerAreaChange data


    }
}
