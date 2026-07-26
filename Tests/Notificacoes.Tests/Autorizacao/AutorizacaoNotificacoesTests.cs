using System.Security.Claims;
using AtronNotificacoes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Notificacoes.Tests.Autorizacao;

public sealed class AutorizacaoNotificacoesTests
{
    [Fact]
    public async Task Publicador_deve_exigir_token_de_servico_e_escopo_de_publicacao()
    {
        using var provider = CriarProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var permitido = await authorizationService.AuthorizeAsync(
            CriarPrincipal(
                new Claim(SegurancaNotificacoes.ClaimTipoToken, SegurancaNotificacoes.TipoTokenServico),
                new Claim(SegurancaNotificacoes.ClaimEscopo, SegurancaNotificacoes.EscopoPublicar)),
            null,
            SegurancaNotificacoes.PoliticaPublicador);
        var semEscopo = await authorizationService.AuthorizeAsync(
            CriarPrincipal(new Claim(SegurancaNotificacoes.ClaimTipoToken, SegurancaNotificacoes.TipoTokenServico)),
            null,
            SegurancaNotificacoes.PoliticaPublicador);

        Assert.True(permitido.Succeeded);
        Assert.False(semEscopo.Succeeded);
    }

    [Fact]
    public async Task Consulta_deve_exigir_usuario_com_codigo_e_rejeitar_token_de_servico()
    {
        using var provider = CriarProvider();
        var authorizationService = provider.GetRequiredService<IAuthorizationService>();

        var usuario = await authorizationService.AuthorizeAsync(
            CriarPrincipal(new Claim(SegurancaNotificacoes.ClaimCodigoUsuario, "USR001")),
            null,
            SegurancaNotificacoes.PoliticaUsuario);
        var servicoComCodigo = await authorizationService.AuthorizeAsync(
            CriarPrincipal(
                new Claim(SegurancaNotificacoes.ClaimCodigoUsuario, "USR001"),
                new Claim(SegurancaNotificacoes.ClaimTipoToken, SegurancaNotificacoes.TipoTokenServico)),
            null,
            SegurancaNotificacoes.PoliticaUsuario);

        Assert.True(usuario.Succeeded);
        Assert.False(servicoComCodigo.Succeeded);
    }

    private static ServiceProvider CriarProvider()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] = "chave-de-teste-do-usuario-com-tamanho-suficiente",
                ["Jwt:Issuer"] = "issuer-usuario",
                ["Jwt:Audience"] = "audience-usuario",
                ["Servico:SecretKey"] = "chave-de-teste-do-servico-com-tamanho-suficiente",
                ["Servico:Issuer"] = "issuer-servico",
                ["Servico:Audience"] = "audience-servico",
                ["Cors:AllowedOrigins:0"] = "https://angular.teste",
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Database=notificacoes;Username=test;Password=test"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddLogging();
        new Startup(configuration).ConfigureServices(services);
        return services.BuildServiceProvider();
    }

    private static ClaimsPrincipal CriarPrincipal(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "teste"));
}
