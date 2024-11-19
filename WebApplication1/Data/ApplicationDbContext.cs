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
        
        
        // DbSet for UserData
        public DbSet<UserData> UserData { get; set; }


        public DbSet<AreaChange> AreaChange { get; set; }
    }
}
