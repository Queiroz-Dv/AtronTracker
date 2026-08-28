using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Application.DTO.Request;
using Application.DTO.Response;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Auth;
using Xunit;

namespace Platform.Tests;

public sealed class EmpresaApiTests
{
    [Fact]
    public async Task RotasEmpresa_DevemRecusarAnonimo()
    {
        using var factory = new EmpresaApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });

        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/Empresa")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
    }

    [Fact]
    public async Task CadastroEConsulta_DevemUsarSomenteOUsuarioAutenticado()
    {
        using var factory = new EmpresaApiFactory();
        using var ana = await factory.CriarClienteAsync("ANA");
        using var bruno = await factory.CriarClienteAsync("BRUNO");

        Assert.Equal(HttpStatusCode.NoContent, (await ana.GetAsync("/api/Empresa")).StatusCode);
        var criado = await ana.PostAsJsonAsync("/api/Empresa", new
        {
            codigo = "Estudo", nomeFantasia = "Empresa da Ana",
            endereco = new { logradouro = "Rua de Teste" }, numero = "(11) 99999-0000",
            email = "empresa@example.test", usuarioCodigo = "BRUNO", usuarioId = 999, empresaId = 999
        });

        Assert.Equal(HttpStatusCode.Created, criado.StatusCode);
        Assert.EndsWith("/api/Empresa", criado.Headers.Location!.ToString());
        var empresa = await criado.Content.ReadFromJsonAsync<EmpresaResponse>();
        Assert.Equal("Estudo", empresa!.Codigo);
        Assert.Equal("Rua de Teste", empresa.Endereco.Logradouro);
        var consulta = await ana.GetAsync("/api/Empresa?usuarioCodigo=BRUNO&empresaId=999");
        Assert.Equal(empresa, await consulta.Content.ReadFromJsonAsync<EmpresaResponse>());
        Assert.Equal(HttpStatusCode.NoContent, (await bruno.GetAsync("/api/Empresa")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await ana.PostAsJsonAsync("/api/Empresa", Request("Outra"))).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await bruno.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await bruno.PostAsJsonAsync("/api/Empresa", Request("EmpresaBruno"))).StatusCode);

        Assert.Equal("Estudo", (await ana.GetFromJsonAsync<EmpresaResponse>("/api/Empresa"))!.Codigo);
        Assert.Equal("EmpresaBruno", (await bruno.GetFromJsonAsync<EmpresaResponse>("/api/Empresa"))!.Codigo);
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
        Assert.Equal(2, await context.Usuarios.CountAsync());
        Assert.Equal(2, await context.Empresas.CountAsync());
        Assert.Equal(2, await context.UsuariosEmpresas.CountAsync());
    }

    [Fact]
    public async Task CadastroInvalido_NaoDevePersistirEmpresaOuVinculo()
    {
        using var factory = new EmpresaApiFactory();
        using var client = await factory.CriarClienteAsync("ANA");
        var request = Request();
        request.Endereco.Logradouro = " ";

        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/Empresa", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.GetAsync("/api/Empresa")).StatusCode);
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
        Assert.Equal(0, await context.Empresas.CountAsync());
        Assert.Equal(0, await context.UsuariosEmpresas.CountAsync());
    }

    private static EmpresaCadastroRequest Request(string codigo = "Estudo") => new()
    {
        Codigo = codigo, NomeFantasia = "Empresa de estudos", Email = "empresa@example.test",
        Numero = "(11) 99999-0000", Endereco = new EnderecoEmpresaRequest { Logradouro = "Rua de Teste" }
    };
}

internal sealed class EmpresaApiFactory : WebApplicationFactory<AtronPlatform.WebApi.Program>
{
    private readonly string _database = Guid.NewGuid().ToString();

    public async Task<HttpClient> CriarClienteAsync(string codigo)
    {
        var client = CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
        context.Usuarios.Add(new Usuario(codigo, codigo, "Teste", $"{codigo}@example.test", null) { EmailConfirmado = true });
        await context.SaveChangesAsync();
        client.DefaultRequestHeaders.Add("X-Usuario-Teste", codigo);
        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:SecretKey"] = "chave-local-exclusiva-dos-testes-empresa-123456789",
            ["ATRON_CONNECTION_STRING"] = "Host=localhost;Database=empresa_tests;Username=tests;Password=tests",
            ["ProcessamentosProdutosLote:WorkerHabilitado"] = "false"
        }));
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<AtronDbContext>>();
            services.RemoveAll<AtronDbContext>();
            services.AddDbContext<AtronDbContext>(options => options.UseInMemoryDatabase(_database));
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "EmpresaTests";
                options.DefaultChallengeScheme = "EmpresaTests";
            }).AddScheme<AuthenticationSchemeOptions, EmpresaTestAuthHandler>("EmpresaTests", _ => { });
        });
    }
}

internal sealed class EmpresaTestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var codigo = Request.Headers["X-Usuario-Teste"].ToString();
        if (string.IsNullOrWhiteSpace(codigo))
            return Task.FromResult(AuthenticateResult.NoResult());

        var identity = new ClaimsIdentity([new Claim(ClaimCode.CODIGO_USUARIO, codigo)], Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}

