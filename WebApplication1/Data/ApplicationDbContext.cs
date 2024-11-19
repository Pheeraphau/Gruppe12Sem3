using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Inkluder nødvendige tabeller
    public DbSet<GeoChange> GeoChanges { get; set; }
    public DbSet<AreaChange> AreaChanges { get; set; }
    public DbSet<PositionModel> Positions { get; set; }
    public DbSet<UserData> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfigurasjon for GeoChange
        modelBuilder.Entity<GeoChange>()
            .HasKey(gc => gc.Id); // Sett Id som primærnøkkel

        // Konfigurasjon for AreaChange
        modelBuilder.Entity<AreaChange>()
            .HasKey(ac => ac.Id); // Sett Id som primærnøkkel

        // Konfigurasjon for PositionModel
        modelBuilder.Entity<PositionModel>()
            .HasKey(pm => pm.Id); // Sett Id som primærnøkkel

        // Konfigurasjon for UserData
        modelBuilder.Entity<UserData>()
            .HasKey(ud => ud.Id); // Sett Id som primærnøkkel
    }
}

