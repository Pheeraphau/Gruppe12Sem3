using Microsoft.EntityFrameworkCore;
using WebApplication1.Models; // This imports the models from the Models folder

namespace WebApplication1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // Define DbSets for the models (tables in the database)
        public DbSet<UserData> Users { get; set; }  // Represents a table of user data
        public DbSet<AreaChange> AreaChanges { get; set; }  // Represents a table for AreaChange
        public DbSet<PositionModel> PositionModel { get; set; }  // Represents a table for PositionModel
    }
}
