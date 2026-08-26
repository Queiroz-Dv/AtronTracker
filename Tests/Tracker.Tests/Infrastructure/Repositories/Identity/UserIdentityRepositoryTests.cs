using AtronTracker.Infrastructure.Context;
using Infrastructure.Repositories.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.Identity;
using Xunit;

namespace Tracker.Tests.Infrastructure.Repositories.Identity;

public sealed class UserIdentityRepositoryTests
{
    [Fact]
    public async Task AtualizarRefreshToken_DevePersistir_QuandoEstadoEstiverAtualizado()
    {
        var options = CriarOptions();
        await using var context = new AtronDbContext(options);
        context.AppUsers.Add(CriarUsuario());
        await context.SaveChangesAsync();

        var repository = new UserIdentityRepository(context, null!);

        var atualizado = await repository.AtualizarRefreshTokenUsuarioRepositoryAsync(
            "USR001",
            "novo-hash",
            DateTime.UtcNow.AddDays(1));

        Assert.True(atualizado);
        Assert.Equal("novo-hash", (await context.AppUsers.SingleAsync()).RefreshToken);
    }

    [Fact]
    public async Task AtualizarRefreshToken_DeveRetornarFalso_QuandoHouverConcorrencia()
    {
        var options = CriarOptions();

        await using (var seedContext = new AtronDbContext(options))
        {
            seedContext.AppUsers.Add(CriarUsuario());
            await seedContext.SaveChangesAsync();
        }

        await using var contextoDesatualizado = new AtronDbContext(options);
        await contextoDesatualizado.AppUsers.SingleAsync();

        await using (var contextoConcorrente = new AtronDbContext(options))
        {
            var usuario = await contextoConcorrente.AppUsers.SingleAsync();
            usuario.RefreshToken = "hash-concorrente";
            await contextoConcorrente.SaveChangesAsync();
        }

        var repository = new UserIdentityRepository(contextoDesatualizado, null!);

        var atualizado = await repository.AtualizarRefreshTokenUsuarioRepositoryAsync(
            "USR001",
            "novo-hash",
            DateTime.UtcNow.AddDays(1));

        Assert.False(atualizado);
    }

    private static DbContextOptions<AtronDbContext> CriarOptions() =>
        new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase($"refresh-token-{Guid.NewGuid()}")
            .Options;

    private static ApplicationUser CriarUsuario() => new()
    {
        Id = 1,
        UserName = "USR001",
        NormalizedUserName = "USR001",
        Email = "usuario@atron.local",
        NormalizedEmail = "USUARIO@ATRON.LOCAL",
        ConcurrencyStamp = "concorrencia-inicial",
        RefreshToken = "hash-inicial"
    };
}
