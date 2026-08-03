using System.Security.Claims;
using AtronNotificacoes;
using AtronNotificacoes.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Notificacoes.Tests.Autorizacao;

public sealed class AutorizacaoNotificacoesTests
{
    [Fact]
    public async Task Consulta_deve_exigir_usuario_autenticado_com_codigo()
    {
        using var provider = CriarProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var usuario = await authorizationService.AuthorizeAsync(
            CriarPrincipal(new Claim(SegurancaNotificacoes.ClaimCodigoUsuario, "USR001")),
            null,
            SegurancaNotificacoes.PoliticaUsuario);
        var usuarioSemCodigo = await authorizationService.AuthorizeAsync(
            CriarPrincipal(),
            null,
            SegurancaNotificacoes.PoliticaUsuario);

        Assert.True(usuario.Succeeded);
        Assert.False(usuarioSemCodigo.Succeeded);
    }

    private static ServiceProvider CriarProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=notificacoes;Username=test;Password=test"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddNotificacoesInternasCapability(configuration);
        return services.BuildServiceProvider();
    }

    private static ClaimsPrincipal CriarPrincipal(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "teste"));
}
