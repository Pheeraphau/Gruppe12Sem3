using System; // For IDisposable
using Microsoft.AspNetCore.Identity; // For UserManager og SignInManager
using Microsoft.AspNetCore.Mvc; // For ViewResult og RedirectToActionResult
using NSubstitute; // For mocking av avhengigheter
using WebApplication1.Controllers; // AccountController som testes
using WebApplication1.Models; // LoginViewModel
using Xunit; // For test-rammeverket
using System.Threading.Tasks; // For async metoder

namespace WebApplication1.Tests.ControllerTests
{
    /// <summary>
    /// Testklasse for AccountController som verifiserer funksjonaliteten til metoder i controlleren.
    /// Funksjonalitet testet:
    /// 1. Login med ugyldige legitimasjonsopplysninger.
    /// 2. Register-metode som returnerer riktig view.
    /// 3. Logout som korrekt redirecter brukeren til Home/Index.
    /// Oppsett:
    /// - Mocker UserManager og SignInManager for å simulere autentisering og brukerstyring.
    /// - Bruker NSubstitute for å opprette mock-objekter.
    /// - Tester er organisert etter Arrange, Act, Assert-mønsteret.
    /// </summary>
    public class AccountControllerTests : IDisposable
    {
        /// <summary>
        /// Oppretter en mock av UserManager for testing.
        /// </summary>
        private UserManager<IdentityUser> CreateMockUserManager()
        {
            return Substitute.For<UserManager<IdentityUser>>(
                Substitute.For<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

        /// <summary>
        /// Oppretter en mock av SignInManager for testing.
        /// </summary>
        private SignInManager<IdentityUser> CreateMockSignInManager(UserManager<IdentityUser> userManager)
        {
            var contextAccessor = Substitute.For<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var claimsFactory = Substitute.For<IUserClaimsPrincipalFactory<IdentityUser>>();

            return Substitute.For<SignInManager<IdentityUser>>(
                userManager,
                contextAccessor,
                claimsFactory,
                null,
                null,
                null,
                null
            );
        }

        /// <summary>
        /// Tester login-metoden for feilaktige legitimasjonsopplysninger.
        /// </summary>
        [Fact]
        public async Task Login_Post_InvalidCredentials_ShouldReturnViewWithError()
        {
            // Arrange
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);
            var model = new LoginViewModel
            {
                Email = "invalid@example.com",
                Password = "InvalidPassword"
            };

            // Mock oppførsel: Returner feil ved pålogging
            signInManager.PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>())
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result); // Sjekk at viewet ikke er null
            Assert.False(controller.ModelState.IsValid); // Sjekk at ModelState er ugyldig
        }

        /// <summary>
        /// Tester Register-metoden for å sjekke at den returnerer riktig view.
        /// </summary>
        [Fact]
        public void Register_ShouldReturnView()
        {
            // Arrange
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);

            // Act
            var result = controller.Register() as ViewResult;

            // Assert
            Assert.NotNull(result); // Sjekk at viewet ikke er null
        }

        /// <summary>
        /// Tester Logout-metoden for å sjekke at brukeren redirectes til Home/Index.
        /// </summary>
        [Fact]
        public async Task Logout_ShouldRedirectToHomeIndex()
        {
            // Arrange
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);

            // Act
            var result = await controller.Logout() as RedirectToActionResult;

            // Assert
            Assert.NotNull(result); // Sjekk at resultatet ikke er null
            Assert.Equal("Index", result.ActionName); // Sjekk at action er \"Index\"
            Assert.Equal("Home", result.ControllerName); // Sjekk at controller er \"Home\"
        }

        // <summary>
        // Rydder opp i ressurser som brukes under testen.
        // For øyeblikket er det ingen eksterne ressurser som krever eksplisitt opprydding,
        // siden vi bruker en in-memory database og mock-objekter.
        // Denne metoden er inkludert for fremtidig fleksibilitet hvis det legges til ressurser
        // som trenger å bli frigjort eksplisitt.
        // </summary>
        public void Dispose()
        {
            // Ingen opprydding nødvendig for in-memory database eller mock-objekter.
            // Legg til oppryddingslogikk her hvis det i fremtiden brukes ressurser som trenger frigjøring.
        }
    }
}
