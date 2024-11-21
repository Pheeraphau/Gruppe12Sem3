using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Data;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Add database context (replace 'DefaultConnection' with your actual connection string name in appsettings.json)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Add Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add Razor Pages if using them
builder.Services.AddControllersWithViews();

// Add session services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Only accessible via HTTP
    options.Cookie.IsEssential = true; // Mark cookie as essential
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Redirect here when not logged in
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect here when access is denied
});

var app = builder.Build();

// Apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate(); // Apply migrations if necessary

    // Seed roles and admin user
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Define roles
    var roles = new[] { "Saksbehandler", "User" }; // Add roles as needed

    foreach (var role in roles)
    {
        if (!roleManager.RoleExistsAsync(role).Result)
        {
            roleManager.CreateAsync(new IdentityRole(role)).Wait();
        }
    }

    // Create default admin user
    var adminEmail = "kristian@testmail.com";
    var adminPassword = "Admin123";
    if (userManager.FindByEmailAsync(adminEmail).Result == null)
    {
        var adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = userManager.CreateAsync(adminUser, adminPassword).Result;

        if (result.Succeeded)
        {
            userManager.AddToRoleAsync(adminUser, "Saksbehandler").Wait();
        }
    }
}

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enforce HTTPS
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Use session middleware
app.UseSession();

// Define default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
