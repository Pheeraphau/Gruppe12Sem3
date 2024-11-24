using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication1.Controllers;
using WebApplication1.Data;
using Microsoft.AspNetCore.Identity;
using NSubstitute;

public class UnitTest1
{
    [Fact]
    public void Test_RegisterAreaChange_ShouldReturnView()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        var mockDbContext = new ApplicationDbContext(options);

        var mockLogger = Substitute.For<ILogger<HomeController>>();
        var mockUserManager = Substitute.For<UserManager<IdentityUser>>(
            Substitute.For<IUserStore<IdentityUser>>(), null, null, null, null, null, null, null, null);

        var controller = new HomeController(mockLogger, mockDbContext, mockUserManager);

        // Act
        var result = controller.RegisterAreaChange();

        // Assert
        Assert.NotNull(result); // Test at resultatet ikke er null
    }
}