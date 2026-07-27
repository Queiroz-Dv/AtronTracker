using Application.Interfaces.Services;
using AtronNotificacoes.Application;
using AtronNotificacoes.Contracts;
using AtronNotificacoes.Infrastructure;
using AtronStock.Application.Interfaces;
using AtronStock.Infrastructure.Context;
using AtronTracker.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Context;
using System.Net;
using Xunit;

namespace Platform.Tests;

public sealed class AtronPlatformHostTests : IClassFixture<AtronPlatformFactory>
{
    private static readonly HashSet<string> ControllersTracker =
    [
        "Acesso",
        "Cargo",
        "Departamento",
        "Modulo",
        "PerfilDeAcesso",
        "PlanejamentoCusto",
        "Sessao",
        "Tarefa",
        "Usuario"
    ];

    private static readonly HashSet<string> ControllersStock =
    [
        "Categoria",
        "Cliente",
        "Estoque",
        "Fornecedor",
        "Produto"
    ];

    private readonly AtronPlatformFactory _factory;
    private readonly HttpClient _client;

    public AtronPlatformHostTests(AtronPlatformFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public void Tracker_DeveSerComposto()
    {
        using var scope = _factory.Services.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<IModuloService>());
        Assert.NotNull(scope.ServiceProvider.GetService<AtronDbContext>());
    }

    [Fact]
    public void Auditoria_DeveSerCompostaComoCapacidadeTransversal()
    {
        using var scope = _factory.Services.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<IAuditoriaService>());
        Assert.NotNull(scope.ServiceProvider.GetService<SharedDbContext>());
    }

    [Fact]
    public void Stock_DeveSerComposto()
    {
        using var scope = _factory.Services.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<ICategoriaService>());
        Assert.NotNull(scope.ServiceProvider.GetService<IEstoqueService>());
        Assert.NotNull(scope.ServiceProvider.GetService<StockDbContext>());
    }

    [Fact]
    public void NotificacoesInternas_DevemSerCompostasComoCapacidadeTransversal()
    {
        using var scope = _factory.Services.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<INotificacaoInternaService>());
        Assert.IsType<NotificacoesInternasInProcessPublisher>(
            scope.ServiceProvider.GetRequiredService<INotificacoesInternasPublisher>());
        Assert.NotNull(scope.ServiceProvider.GetService<NotificacoesDbContext>());
    }

    [Fact]
    public async Task Saude_DeveResponderComSucesso()
    {
        var response = await _client.GetAsync("/api/saude");

        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task Swagger_DeveSerDisponibilizado()
    {
        var response = await _client.GetAsync("/swagger/v1/swagger.json");

        await AssertStatusAsync(response, HttpStatusCode.OK);
    }

    [Fact]
    public async Task RotaTarefa_DeveSerPublicadaEManterProtecao()
    {
        var response = await _client.GetAsync("/api/Tarefa");

        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RotaAuditoria_DeveExigirAutenticacao()
    {
        var response = await _client.GetAsync("/Auditoria/registro/contexto");

        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void JwtBearer_DeveSerOEsquemaPadraoDoHost()
    {
        var options = _factory.Services
            .GetRequiredService<IOptions<AuthenticationOptions>>()
            .Value;

        Assert.Equal(
            JwtBearerDefaults.AuthenticationScheme,
            options.DefaultAuthenticateScheme);
        Assert.Equal(
            JwtBearerDefaults.AuthenticationScheme,
            options.DefaultChallengeScheme);
    }

    [Fact]
    public async Task RotaCategoria_DeveSerPublicadaEManterProtecao()
    {
        var response = await _client.GetAsync("/api/Categoria");

        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RotaNotificacoes_DeveSerPublicadaEManterProtecao()
    {
        var response = await _client.GetAsync("/api/notificacoes");

        await AssertStatusAsync(response, HttpStatusCode.Unauthorized);
    }

    [Fact]
    public void RotasDoTracker_DevemPermanecerPublicadasNoHostNeutro()
    {
        var contratos = ObterContratosDoTracker(_factory.Services);
        var controllers = contratos
            .Select(contrato => contrato.Controller)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(66, contratos.Count);
        Assert.True(ControllersTracker.SetEquals(controllers));
    }

    [Fact]
    public void RotaDaAuditoria_DevePermanecerPublicadaNoHostNeutro()
    {
        var contratos = ObterContratos(
            _factory.Services,
            new HashSet<string>(StringComparer.Ordinal)
            {
                "Auditoria"
            });

        var contrato = Assert.Single(contratos);
        Assert.Equal("Auditoria", contrato.Controller);
        Assert.Equal("GET", contrato.Metodos);
        Assert.False(contrato.PermiteAnonimo);
        Assert.NotEmpty(contrato.Autorizacao);
    }

    [Fact]
    public void RotasDeNotificacoes_DevemPermanecerPublicadasNoHostNeutro()
    {
        var contratos = ObterContratos(
            _factory.Services,
            new HashSet<string>(StringComparer.Ordinal)
            {
                "NotificacoesInternas",
                "ProntidaoNotificacoes"
            });

        Assert.Equal(5, contratos.Count);
        Assert.All(contratos, contrato =>
        {
            Assert.False(contrato.PermiteAnonimo);
            Assert.NotEmpty(contrato.Autorizacao);
        });
    }

    [Fact]
    public void RotasDoStock_DevemPermanecerPublicadasNoHostNeutro()
    {
        var contratos = ObterContratos(_factory.Services, ControllersStock);
        var controllers = contratos
            .Select(contrato => contrato.Controller)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(19, contratos.Count);
        Assert.True(ControllersStock.SetEquals(controllers));
    }

    private static async Task AssertStatusAsync(
        HttpResponseMessage response,
        HttpStatusCode expectedStatus)
    {
        var content = await response.Content.ReadAsStringAsync();

        Assert.True(
            response.StatusCode == expectedStatus,
            $"Status esperado: {expectedStatus}. Status recebido: {response.StatusCode}. Corpo: {content}");
    }

    private static IReadOnlyList<EndpointContrato> ObterContratosDoTracker(
        IServiceProvider services)
    {
        return ObterContratos(services, ControllersTracker);
    }

    private static IReadOnlyList<EndpointContrato> ObterContratos(
        IServiceProvider services,
        IReadOnlySet<string> controllers)
    {
        return services
            .GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => new
            {
                Endpoint = endpoint,
                Acao = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>()
            })
            .Where(item =>
                item.Acao is not null &&
                controllers.Contains(item.Acao.ControllerName))
            .Select(item =>
            {
                var autorizacoes = item.Endpoint.Metadata
                    .OfType<IAuthorizeData>()
                    .Select(authorization =>
                        $"{authorization.Policy}|{authorization.Roles}|{authorization.AuthenticationSchemes}")
                    .OrderBy(value => value);

                var metodos = item.Endpoint.Metadata
                    .GetMetadata<HttpMethodMetadata>()?
                    .HttpMethods
                    .OrderBy(method => method)
                    .ToArray()
                    ?? Array.Empty<string>();

                return new EndpointContrato(
                    item.Acao!.ControllerName,
                    item.Acao.ActionName,
                    item.Endpoint.RoutePattern.RawText ?? string.Empty,
                    string.Join(",", metodos),
                    string.Join(";", autorizacoes),
                    item.Endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null);
            })
            .Distinct()
            .OrderBy(endpoint => endpoint.Controller)
            .ThenBy(endpoint => endpoint.Rota)
            .ThenBy(endpoint => endpoint.Metodos)
            .ToList();
    }

    private sealed record EndpointContrato(
        string Controller,
        string Acao,
        string Rota,
        string Metodos,
        string Autorizacao,
        bool PermiteAnonimo);
}

public sealed class AtronPlatformFactory : WebApplicationFactory<AtronPlatform.WebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] =
                    "chave-local-para-testes-do-host-neutro-123456789",
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=atron_platform_tests;Username=tests;Password=tests"
            });
        });
    }
}
