using AtronTracker.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Infrastructure.Authorization;

public sealed class ModuloEmpresaTests
{
    [Fact]
    public async Task Modelo_DeveSemearModuloEmpresa()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase($"modulo-empresa-{Guid.NewGuid()}")
            .Options;

        await using var context = new AtronDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var modulo = await context.Modulos.SingleAsync(item => item.Codigo == "EMP");

        Assert.Equal(14, modulo.Id);
        Assert.Equal("Empresas", modulo.Descricao);
    }
}
