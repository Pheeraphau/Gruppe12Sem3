/*
 * AccountControllerTests - Enhetstester for AccountController
 *
 * Denne testklassen inneholder følgende tester:
 * 
 * 1. Login_Post_InvalidCredentials_ShouldReturnViewWithError:
 *    Tester om login-metoden håndterer feilaktige legitimasjon korrekt ved å returnere riktig view og legge til en feilmelding i ModelState.
 *    - Bruker mock av UserManager og SignInManager for å simulere feil pålogging.
 *
 * 2. Register_ShouldReturnView:
 *    Tester om Register-metoden returnerer riktig view når den blir kalt uten data (GET-metode).
 *    - Bruker mock av UserManager og SignInManager for å sette opp AccountController.
 *
 * 3. Logout_ShouldRedirectToHomeIndex:
 *    Tester om Logout-metoden logger ut brukeren og redirecter til Home/Index-siden.
 *    - Bruker mock av UserManager og SignInManager for å simulere logout-prosessen.
 *
 * Mock-objekter:
 * - UserManager<IdentityUser>: Mockes for å simulere brukerrelaterte operasjoner som opprettelse, pålogging og tildeling av roller.
 * - SignInManager<IdentityUser>: Mockes for å simulere autentisering og utlogging av brukere.
 *
 * Verktøy:
 * - NSubstitute: Brukes for å opprette mock-objekter.
 * - xUnit: Brukes for å definere og kjøre testene.
 *
 * Struktur:
 * - Testene er organisert etter Arrange, Act, Assert-mønsteret for å gjøre koden lesbar og enkel å forstå.
 */

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using WebApplication1.Controllers;
using WebApplication1.Models;
using Xunit;
using System.Threading.Tasks; // For Task

namespace WebApplication1.Tests.ControllerTests
{
    public class AccountControllerTests
    {
        // Oppretter en mock av UserManager for testing
        private UserManager<IdentityUser> CreateMockUserManager()
        {
            return Substitute.For<UserManager<IdentityUser>>(
                Substitute.For<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );
        }
       // Oppretter en mock av SignInManager for testing
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

        // Test 1: Tester login-metoden for å sjekke at feil pålogging returnerer riktig view med en feilmelding
        [Fact]
        public async Task Login_Post_InvalidCredentials_ShouldReturnViewWithError()
        {
            // Arrange: Setter opp mockeobjekter og en controller
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);
            var model = new LoginViewModel
            {
                Email = "invalid@example.com",
                Password = "InvalidPassword"
            };
            // Mock oppførsel: Returnerer feil ved pålogging
            signInManager.PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>())
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act metoden: Kaller login-metoden med feil legitimasjon
            var result = await controller.Login(model) as ViewResult;

            // Assert: Sjekker at resultatet ikke er null og at modellen har feil
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

        // Test 2: Tester om Register-metoden returnerer riktig view
        [Fact]
        public void Register_ShouldReturnView()
        {
            // Arrange: Setter opp mockeobjekter og en controller
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);

            // Act: Kaller Register-metoden
            var result = controller.Register() as ViewResult;

            // Assert: Sjekker at resultatet ikke er null
            Assert.NotNull(result);
        }

        // Test 3: Tester Logout-metoden for å sjekke at den redirecter til Home/Index
        [Fact]
        public async Task Logout_ShouldRedirectToHomeIndex()
        {
            // Arrange: Setter opp mockeobjekter og en controller
            var userManager = CreateMockUserManager();
            var signInManager = CreateMockSignInManager(userManager);
            var controller = new AccountController(userManager, signInManager);

            // Act: Kaller Logout-metoden
            var result = await controller.Logout() as RedirectToActionResult;

            // Assert: Sjekker at redirecten er riktig
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }
    }
}
