using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Database-sett for GeoChange-objekter
        public DbSet<GeoChange> GeoChanges { get; set; }

        // Database-sett for UserData-objekter
        public DbSet<UserData> UserData { get; set; }

        // Database-sett for posisjoner (PositionModel-objekter)
        public DbSet<PositionModel> Positions { get; set; }

        // Database-sett for AreaChange-objekter
        public DbSet<AreaChange> AreaChange { get; set; }
    }
}