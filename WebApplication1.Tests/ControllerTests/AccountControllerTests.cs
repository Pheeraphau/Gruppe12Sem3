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
        private UserManager<IdentityUser> CreateMockUserManager()
        {
            return Substitute.For<UserManager<IdentityUser>>(
                Substitute.For<IUserStore<IdentityUser>>(),
                null, null, null, null, null, null, null, null
            );
        }

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

            signInManager.PasswordSignInAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<bool>(), Arg.Any<bool>())
                .Returns(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await controller.Login(model) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.False(controller.ModelState.IsValid);
        }

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
            Assert.NotNull(result);
        }

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
            Assert.NotNull(result);
            Assert.Equal("Index", result.ActionName);
            Assert.Equal("Home", result.ControllerName);
        }
    }
}
