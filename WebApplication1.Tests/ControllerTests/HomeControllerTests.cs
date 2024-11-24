using Xunit; // xUnit brukes som test-rammeverk
using Microsoft.EntityFrameworkCore; // For å sette opp en in-memory database
using Microsoft.Extensions.Logging; // For å logge, mockes i testen
using WebApplication1.Controllers; // HomeController som skal testes
using WebApplication1.Data; // ApplicationDbContext, database-konteksten
using Microsoft.AspNetCore.Identity; // UserManager for håndtering av brukere
using NSubstitute; // For å mocke avhengigheter

/// <summary>
/// Denne testen verifiserer at metoden RegisterAreaChange i HomeController
/// returnerer et gyldig resultat. Det brukes en in-memory database og mockede 
/// avhengigheter (ILogger og UserManager) for å unngå eksterne avhengigheter.
/// </summary>
public class HomeControllerTests
{
    [Fact] // Marker testen som en enhetstest
    public void Test_RegisterAreaChange_ShouldReturnView()
    {
        // Arrange - forbered testmiljøet

        // Sett opp en in-memory database for å unngå avhengighet til en ekte database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase") // Angir databasenavnet
            .Options;
        var mockDbContext = new ApplicationDbContext(options); // Opprett databasekontekst

        // Mock logger for HomeController
        var mockLogger = Substitute.For<ILogger<HomeController>>();

        // Mock UserManager for håndtering av brukere
        var mockUserManager = Substitute.For<UserManager<IdentityUser>>(
            Substitute.For<IUserStore<IdentityUser>>(), // Mock av IUserStore
            null, null, null, null, null, null, null, null // Setter null for unødvendige parametere
        );

        // Opprett controller med mockede avhengigheter
        var controller = new HomeController(mockLogger, mockDbContext, mockUserManager);

        // Act - utfør handlingen som testes
        var result = controller.RegisterAreaChange(); // Kall metoden som skal testes

        // Assert - bekreft forventet oppførsel
        Assert.NotNull(result); // Verifiser at resultatet ikke er null
    }
}
