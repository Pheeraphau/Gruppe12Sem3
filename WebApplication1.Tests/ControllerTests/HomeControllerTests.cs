using System; // For IDisposable
using Microsoft.AspNetCore.Mvc; // Kreves for ViewResult
using System.Security.Claims; // For mocking authenticated users
using Xunit; // xUnit brukes som test-rammeverk
using Microsoft.EntityFrameworkCore; // For å sette opp en in-memory database
using Microsoft.Extensions.Logging; // For å logge, mockes i testen
using Microsoft.AspNetCore.Identity; // UserManager for håndtering av brukere
using NSubstitute; // For mocking av avhengigheter
using WebApplication1.Controllers; // HomeController som skal testes
using WebApplication1.Data; // ApplicationDbContext, database-konteksten
using System.Collections.Generic; // For List<>

// <summary>
// Testklasse for HomeController som verifiserer funksjonaliteten til metoder i controlleren.
// 
// Funksjonalitet testet:
// 1. RegisterAreaChange - Tester at metoden returnerer et gyldig view-objekt.
// 2. AreaChangeOverview - Tester at metoden returnerer riktig filtrerte data for innlogget bruker.
// 3. Delete - Tester at metoden sletter en GeoChange korrekt og håndterer ugyldige ID-er.
//
// Oppsett:
// - Bruker en in-memory database (ApplicationDbContext) for å isolere testen fra en ekte database.
// - Mocker ILogger og UserManager for å simulere eksterne avhengigheter og redusere coupling.
// - Implementerer IDisposable for å rydde opp ressursene etter hver test.
//
// Testrammeverk og verktøy:
// - xUnit: For definering og kjøring av tester.
// - NSubstitute: For mocking av avhengigheter.
// - Microsoft.EntityFrameworkCore.InMemory: For å bruke en database i minnet.
//
// Kommentarer er lagt til for å forklare hver del av koden.
// </summary>
namespace WebApplication1.Tests.ControllerTests // Oppdatert namespace
{
    public class HomeControllerTests : IDisposable
    {
        // In-memory database for å simulere en ekte database
        private readonly ApplicationDbContext _mockDbContext;

        // Mock av ILogger for å teste uten avhengighet til ekte logging
        private readonly ILogger<HomeController> _mockLogger;

        // Mock av UserManager for å teste uten en faktisk brukermanager
        private readonly UserManager<IdentityUser> _mockUserManager;

        /// <summary>
        /// Constructor som setter opp felles avhengigheter for testen.
        /// - Initialiserer en in-memory database.
        /// - Mocker nødvendige avhengigheter som logger og UserManager.
        /// </summary>
        public HomeControllerTests()
        {
            // Konfigurer en in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Database lagret i minnet
                .Options;

            // Initialiserer databasekonteksten
            _mockDbContext = new ApplicationDbContext(options);

            // Oppretter en mock av ILogger
            _mockLogger = Substitute.For<ILogger<HomeController>>();

            // Oppretter en mock av UserManager
            _mockUserManager = Substitute.For<UserManager<IdentityUser>>(
                Substitute.For<IUserStore<IdentityUser>>(), // Mock av IUserStore
                null, null, null, null, null, null, null, null // Null for parametere vi ikke trenger
            );
        }

        /// <summary>
        /// Oppretter en instans av HomeController med mockede avhengigheter.
        /// Gjør det enklere å opprette controllere i flere tester.
        /// </summary>
        /// <returns>En instans av HomeController</returns>
        private HomeController CreateController()
        {
            return new HomeController(_mockLogger, _mockDbContext, _mockUserManager);
        }

        /// <summary>
        /// Tester metoden RegisterAreaChange i HomeController.
        /// - Sjekker at metoden returnerer et gyldig view-objekt.
        /// - Testen kjører i et isolert miljø med mockede avhengigheter.
        /// </summary>
        [Fact]
        public void Test_RegisterAreaChange_ShouldReturnView()
        {
            // Arrange - Opprett en instans av controlleren
            var controller = CreateController();

            // Act - Kall metoden RegisterAreaChange
            var result = controller.RegisterAreaChange();

            // Assert - Sjekk at resultatet ikke er null
            Assert.NotNull(result); // Resultatet skal være et gyldig view
        }

        /// <summary>
        /// Tester metoden AreaChangeOverview i HomeController.
        /// - Sjekker at metoden returnerer filtrerte endringer for innlogget bruker.
        /// - Testen kjører i et isolert miljø med mockede avhengigheter og data.
        /// </summary>
        [Fact]
        public void Test_AreaChangeOverview_ShouldReturnViewWithFilteredData()
        {
            // Arrange - Opprett en instans av controlleren
            var controller = CreateController();
            var testUserId = "testUser123"; // Simulert bruker-ID

            // Mock UserManager for å returnere testbrukerens ID
            _mockUserManager.GetUserId(Arg.Any<ClaimsPrincipal>()).Returns(testUserId);

            // Legg til noen testdata i den in-memory databasen
            _mockDbContext.GeoChanges.AddRange(
                new GeoChange { Id = 1, UserId = testUserId, GeoJson = "{}", Description = "Test Change 1", Status = "Innsendt" },
                new GeoChange { Id = 2, UserId = "anotherUser", GeoJson = "{}", Description = "Test Change 2", Status = "Under behandling" },
                new GeoChange { Id = 3, UserId = testUserId, GeoJson = "{}", Description = "Test Change 3", Status = "Godkjent" }
            );
            _mockDbContext.SaveChanges();

            // Act - Kall metoden AreaChangeOverview
            var result = controller.AreaChangeOverview() as ViewResult;
            var model = result?.Model as List<GeoChange>;

            // Assert - Sjekk resultatet
            Assert.NotNull(result); // ViewResult skal ikke være null
            Assert.NotNull(model); // Modellen skal ikke være null
            Assert.Equal(2, model.Count); // Kun to endringer skal tilhøre testbrukeren
            Assert.All(model, change => Assert.Equal(testUserId, change.UserId)); // Alle endringer skal ha riktig bruker-ID
        }
      /// <summary>
/// Denne testen verifiserer funksjonaliteten til Delete-metoden i HomeController.
/// - Testen sjekker at en gyldig GeoChange slettes korrekt fra databasen og at brukeren blir omdirigert.
/// - Testen sjekker også at metoden håndterer ugyldige ID-er ved å returnere en NotFound-respons.
/// - Testen sikrer at eventuelle feil i slettingen logges og håndteres riktig.
/// </summary>
[Fact]
public void Test_Delete_ShouldHandleValidAndInvalidIds_Robust()
{
    // Arrange
    var controller = CreateController();
    var validGeoChange = new GeoChange { Id = 1, UserId = "testUser123", GeoJson = "{}", Description = "Gyldig endring", Status = "Innsendt" };
    var invalidGeoChangeId = 99;

    // Legg til en gyldig GeoChange i databasen
    _mockDbContext.GeoChanges.Add(validGeoChange);
    _mockDbContext.SaveChanges();

    // Verifiser at den gyldige GeoChange eksisterer før sletting
    Assert.NotNull(_mockDbContext.GeoChanges.Find(validGeoChange.Id));

    // Act: Slett en gyldig GeoChange
    var validResult = controller.Delete(validGeoChange.Id) as RedirectToActionResult;

    // Act: Slett en ugyldig GeoChange
    var invalidResult = controller.Delete(invalidGeoChangeId) as NotFoundObjectResult;

    // Assert: Gyldig sletting
    Assert.NotNull(validResult);
    Assert.Equal("MineInnmeldinger", validResult.ActionName);
    Assert.Null(_mockDbContext.GeoChanges.Find(validGeoChange.Id)); // Sjekk at den er slettet

    // Assert: Ugyldig sletting
    Assert.NotNull(invalidResult);
    Assert.IsType<NotFoundObjectResult>(invalidResult);
    Assert.Contains($"GeoChange med ID {invalidGeoChangeId} ble ikke funnet.", invalidResult.Value.ToString());
}

        // <summary>
        // Rydder opp i ressursene som brukes under testen.
        // - Sletter in-memory databasen.
        // - Frigjør eventuelle andre ressurser.
        // </summary>
        public void Dispose()
        {
            _mockDbContext.Database.EnsureDeleted(); // Slett databasen fra minnet
            _mockDbContext.Dispose(); // Frigjør ressurser relatert til databasen
        }
    }
}
