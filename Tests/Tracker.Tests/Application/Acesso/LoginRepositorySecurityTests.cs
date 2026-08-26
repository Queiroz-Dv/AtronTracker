using Infrastructure.Repositories.ApplicationRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities.Identity;
using Xunit;

namespace Tracker.Tests.Acesso;

public class LoginRepositorySecurityTests
{
    [Fact]
    public async Task SenhaIncorreta_DeveRegistrarFalhaSemCriarCookieOuAlterarRefreshToken()
    {
        var usuario = new ApplicationUser
        {
            UserName = "USR001",
            LockoutEnabled = true
        };
        var userManager = CriarUserManager();
        userManager.Setup(manager => manager.FindByNameAsync("USR001")).ReturnsAsync(usuario);
        userManager.Setup(manager => manager.IsLockedOutAsync(usuario)).ReturnsAsync(false);
        userManager.Setup(manager => manager.CheckPasswordAsync(usuario, "senha-incorreta")).ReturnsAsync(false);
        userManager.Setup(manager => manager.AccessFailedAsync(usuario)).ReturnsAsync(IdentityResult.Success);
        var authManager = new Mock<IAuthManagerService>();
        authManager.SetupGet(manager => manager.UserManager).Returns(userManager.Object);
        var repository = new LoginRepository(authManager.Object);

        var resultado = await repository.ValidarCredenciaisAsync("USR001", "senha-incorreta");

        Assert.False(resultado);
        userManager.Verify(manager => manager.AccessFailedAsync(usuario), Times.Once);
        authManager.VerifyGet(manager => manager.SignInManager, Times.Never);
    }

    [Fact]
    public async Task SenhaCorreta_DeveZerarFalhasSemPersistirTokenDuranteValidacao()
    {
        var usuario = new ApplicationUser
        {
            UserName = "USR001",
            LockoutEnabled = true
        };
        var userManager = CriarUserManager();
        userManager.Setup(manager => manager.FindByNameAsync("USR001")).ReturnsAsync(usuario);
        userManager.Setup(manager => manager.IsLockedOutAsync(usuario)).ReturnsAsync(false);
        userManager.Setup(manager => manager.CheckPasswordAsync(usuario, "senha-correta")).ReturnsAsync(true);
        userManager.Setup(manager => manager.ResetAccessFailedCountAsync(usuario)).ReturnsAsync(IdentityResult.Success);
        var authManager = new Mock<IAuthManagerService>();
        authManager.SetupGet(manager => manager.UserManager).Returns(userManager.Object);
        var repository = new LoginRepository(authManager.Object);

        var resultado = await repository.ValidarCredenciaisAsync("USR001", "senha-correta");

        Assert.True(resultado);
        userManager.Verify(manager => manager.ResetAccessFailedCountAsync(usuario), Times.Once);
        authManager.VerifyGet(manager => manager.SignInManager, Times.Never);
    }

    private static Mock<UserManager<ApplicationUser>> CriarUserManager()
    {
        var store = Mock.Of<IUserStore<ApplicationUser>>();
        return new Mock<UserManager<ApplicationUser>>(
            store,
            Options.Create(new IdentityOptions()),
            Mock.Of<IPasswordHasher<ApplicationUser>>(),
            Array.Empty<IUserValidator<ApplicationUser>>(),
            Array.Empty<IPasswordValidator<ApplicationUser>>(),
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<ApplicationUser>>>());
    }
}
