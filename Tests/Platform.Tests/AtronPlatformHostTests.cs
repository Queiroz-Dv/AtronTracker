using Application.Interfaces.Services;
using Application.UseCases.TarefaCases;
using Application.UseCases.TarefaCases.Movimentacao;
using AtronNotificacoes.Infrastructure;
using AtronStock.Application.Interfaces;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Infrastructure.Context;
using AtronStock.Infrastructure;
using AtronStock.Infrastructure.Workers;
using AtronTracker.Infrastructure.Context;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Application.DTOS.Auth;
using Shared.Infrastructure.Context;
using AtronPlatform.WebApi.Security;
using Application.DTO.Request;
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Xunit;
using AtronNotificacoes.Contracts.Interfaces;
using AtronNotificacoes.Application.Interfaces;
using Shared.Authorization;

namespace Platform.Tests;

public sealed class AtronPlatformHostTests : IClassFixture<AtronPlatformFactory>
{
    private static readonly HashSet<string> ControllersTracker =
    [
        "Acesso",
        "Cargo",
        "Departamento",
        "Empresa",
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
        "Produto",
        "ProcessamentoProduto"
    ];

    private static readonly HashSet<(string Controller, string Acao)> OperacoesPublicasEsperadas =
    [
        ("Acesso", "Login"),
        ("Acesso", "Refresh"),
        ("Acesso", "ReenviarConfirmacaoEmail"),
        ("Acesso", "TrocarSenha"),
        ("Acesso", "RecuperarSenha"),
        ("Acesso", "Post"),
        ("Acesso", "ConfirmarEmail"),
        ("Acesso", "SolicitarReativacao"),
        ("Acesso", "ReativarConta"),
        ("Usuario", "ConfirmarAlteracaoEmail")
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
    public void CasosDeTarefaConsumidos_DevemSerResolvidosNoHostReal()
    {
        using var scope = _factory.Services.CreateScope();
        Type[] casosDeTarefa =
        [
            typeof(AssumirTarefaCase),
            typeof(AtualizarTarefaCase),
            typeof(AtualizarTarefaMovimentacaoCase),
            typeof(CriarTarefaCase),
            typeof(CriarTarefaMovimentacaoCase),
            typeof(DecidirTarefaCase),
            typeof(ExcluirTarefaCase),
            typeof(ObterAcessoTarefaCase),
            typeof(ObterEquipeCase),
            typeof(ObterHistoricoTarefaCase),
            typeof(ObterMeuQuadroCase),
            typeof(ObterSolicitacaoCase),
            typeof(ObterTarefasDisponiveisCase),
            typeof(ObterTarefaCase),
            typeof(RegistrarDecisaoTarefaMovimentacaoCase),
            typeof(RegistrarObtencaoTarefaMovimentacaoCase),
            typeof(RegistrarSolicitacaoTarefaMovimentacaoCase),
            typeof(SolicitarTarefaCase),
            typeof(TarefaNotificacaoInternaCase)
        ];

        foreach (var tipo in casosDeTarefa)
            Assert.NotNull(scope.ServiceProvider.GetRequiredService(tipo));
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
    public void Stock_DeveRegistrarWorkerSingletonEProcessadorScoped()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=atron_platform_tests;Username=tests;Password=tests",
                ["ProcessamentosProdutosLote:WorkerHabilitado"] = "true"
            })
            .Build();

        services.AddStockModule(configuration);

        var worker = Assert.Single(services, descriptor =>
            descriptor.ServiceType == typeof(IHostedService)
            && descriptor.ImplementationType == typeof(GeracaoProdutosLoteWorker));
        Assert.Equal(ServiceLifetime.Singleton, worker.Lifetime);
        var processador = Assert.Single(services, descriptor =>
            descriptor.ServiceType == typeof(ProcessadorProdutosLote));
        Assert.Equal(ServiceLifetime.Scoped, processador.Lifetime);
        var criarLote = Assert.Single(services, descriptor =>
            descriptor.ServiceType == typeof(CriarLoteParaPersistenciaCase));
        Assert.Equal(ServiceLifetime.Scoped, criarLote.Lifetime);
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
    public void Identity_DeveBloquearLoginAposCincoFalhas()
    {
        var options = _factory.Services
            .GetRequiredService<IOptions<IdentityOptions>>()
            .Value;

        Assert.True(options.Lockout.AllowedForNewUsers);
        Assert.Equal(5, options.Lockout.MaxFailedAccessAttempts);
        Assert.Equal(TimeSpan.FromMinutes(15), options.Lockout.DefaultLockoutTimeSpan);
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
    public async Task Login_DeveAplicarRateLimitComRetryAfter()
    {
        HttpResponseMessage? response = null;

        for (var tentativa = 0; tentativa < 6; tentativa++)
        {
            response = await _client.PostAsync(
                "/api/Acesso/Login",
                new StringContent("{", Encoding.UTF8, "application/json"));
        }

        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.TooManyRequests, response.StatusCode);
        Assert.True(response.Headers.Contains("Retry-After"));
        Assert.Contains(
            AuthResource.Erro_MuitasTentativas,
            await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Logout_DeveSerPostEExigirAutenticacao()
    {
        var post = await _client.PostAsync("/api/Acesso/Desconectar", content: null);
        var get = await _client.GetAsync("/api/Acesso/Desconectar");

        Assert.Equal(HttpStatusCode.Unauthorized, post.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, get.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_DeveSerPost()
    {
        var post = await _client.PostAsync("/api/Acesso/RefreshToken", content: null);
        var get = await _client.GetAsync("/api/Acesso/RefreshToken");

        Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
        Assert.Equal(HttpStatusCode.MethodNotAllowed, get.StatusCode);
    }

    [Fact]
    public void OperacoesPublicasDeAcesso_DevemTerRateLimitEspecifico()
    {
        var politicasEsperadas = new Dictionary<string, string>
        {
            ["Login"] = AcessoRateLimiting.Login,
            ["ReenviarConfirmacaoEmail"] = AcessoRateLimiting.ReenvioConfirmacao,
            ["TrocarSenha"] = AcessoRateLimiting.TrocaSenha,
            ["RecuperarSenha"] = AcessoRateLimiting.RecuperacaoSenha,
            ["Post"] = AcessoRateLimiting.Registro,
            ["ConfirmarEmail"] = AcessoRateLimiting.ConfirmacaoEmail,
            ["SolicitarReativacao"] = AcessoRateLimiting.Reativacao,
            ["ReativarConta"] = AcessoRateLimiting.Reativacao
        };

        var politicasAtuais = _factory.Services
            .GetServices<EndpointDataSource>()
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => new
            {
                Acao = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>(),
                RateLimit = endpoint.Metadata.GetMetadata<EnableRateLimitingAttribute>()
            })
            .Where(item =>
                item.Acao?.ControllerName == "Acesso" &&
                politicasEsperadas.ContainsKey(item.Acao.ActionName))
            .ToDictionary(
                item => item.Acao!.ActionName,
                item => item.RateLimit?.PolicyName);

        Assert.Equal(politicasEsperadas.Count, politicasAtuais.Count);
        Assert.All(politicasEsperadas, politica =>
            Assert.Equal(politica.Value, politicasAtuais[politica.Key]));
    }

    [Fact]
    public void RotasDoTracker_DevemPermanecerPublicadasNoHostNeutro()
    {
        var contratos = ObterContratosDoTracker(_factory.Services);
        var controllers = contratos
            .Select(contrato => contrato.Controller)
            .ToHashSet(StringComparer.Ordinal);

        Assert.Equal(71, contratos.Count);
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

        Assert.Equal(26, contratos.Count);
        Assert.True(ControllersStock.SetEquals(controllers));

        var contratosProduto = contratos
            .Where(contrato => contrato.Controller == "Produto")
            .Select(contrato => (contrato.Metodos, contrato.Rota))
            .ToHashSet();
        Assert.True(contratosProduto.SetEquals(
        [
            ("GET", "api/Produto"),
            ("POST", "api/Produto"),
            ("POST", "api/Produto/lotes"),
            ("GET", "api/Produto/{codigo}"),
            ("PUT", "api/Produto/{codigo}")
        ]));
        Assert.All(
            contratos.Where(contrato =>
                contrato.Controller is "Produto" or "ProcessamentoProduto"),
            contrato => Assert.Contains(ModuloPolicies.Produto, contrato.Autorizacao));

        var contratosProcessamento = contratos
            .Where(contrato => contrato.Controller == "ProcessamentoProduto")
            .Select(contrato => (contrato.Metodos, contrato.Rota))
            .ToHashSet();
        Assert.True(contratosProcessamento.SetEquals(
        [
            ("GET", "api/processamentos-produtos"),
            ("GET", "api/processamentos-produtos/{id:int}")
        ]));
    }

    [Fact]
    public void RotasOperacionais_DevemExigirAutenticacao()
    {
        var controllers = ControllersTracker
            .Concat(ControllersStock)
            .Concat(["Auditoria", "NotificacoesInternas", "ProntidaoNotificacoes"])
            .ToHashSet(StringComparer.Ordinal);

        var contratos = ObterContratos(_factory.Services, controllers);
        var desprotegidas = contratos
            .Where(contrato =>
                !OperacoesPublicasEsperadas.Contains((contrato.Controller, contrato.Acao)) &&
                string.IsNullOrWhiteSpace(contrato.Autorizacao))
            .Select(contrato => $"{contrato.Metodos} {contrato.Rota}")
            .ToArray();

        Assert.True(
            desprotegidas.Length == 0,
            $"Rotas operacionais sem autorizacao: {string.Join(", ", desprotegidas)}");

        var operacoesPublicadas = contratos
            .Select(contrato => (contrato.Controller, contrato.Acao))
            .ToHashSet();
        Assert.Subset(operacoesPublicadas, OperacoesPublicasEsperadas);

        var quantidadeRotasAutenticadas = contratos.Count(contrato =>
            !OperacoesPublicasEsperadas.Contains((contrato.Controller, contrato.Acao)));
        Assert.True(
            quantidadeRotasAutenticadas >= 60,
            "O inventario deve comprovar pelo menos 60 rotas autenticadas.");
    }

    [Fact]
    public async Task Cors_DevePermitirSomenteOrigemConfigurada()
    {
        using var origemPermitida = new HttpRequestMessage(HttpMethod.Options, "/api/Tarefa");
        origemPermitida.Headers.Add("Origin", "http://localhost:4200");
        origemPermitida.Headers.Add("Access-Control-Request-Method", "GET");

        using var origemNaoPermitida = new HttpRequestMessage(HttpMethod.Options, "/api/Tarefa");
        origemNaoPermitida.Headers.Add("Origin", "https://origem-nao-permitida.test");
        origemNaoPermitida.Headers.Add("Access-Control-Request-Method", "GET");

        var respostaPermitida = await _client.SendAsync(origemPermitida);
        var respostaNaoPermitida = await _client.SendAsync(origemNaoPermitida);

        Assert.Equal(HttpStatusCode.NoContent, respostaPermitida.StatusCode);
        Assert.Equal(
            "http://localhost:4200",
            Assert.Single(respostaPermitida.Headers.GetValues("Access-Control-Allow-Origin")));
        Assert.Equal(
            "true",
            Assert.Single(respostaPermitida.Headers.GetValues("Access-Control-Allow-Credentials")));
        Assert.False(respostaNaoPermitida.Headers.Contains("Access-Control-Allow-Origin"));
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

public sealed class AtronPlatformProductionHostTests : IClassFixture<AtronPlatformProductionFactory>
{
    private readonly HttpClient _client;

    public AtronPlatformProductionHostTests(AtronPlatformProductionFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Theory]
    [InlineData("/swagger")]
    [InlineData("/swagger/v1/swagger.json")]
    [InlineData("/docs")]
    public async Task DocumentacaoDaApi_NaoDeveSerPublicada(string rota)
    {
        var response = await _client.GetAsync(rota);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RespostasEmProducao_DevemConterCabecalhosDeSeguranca()
    {
        var response = await _client.GetAsync("/rota-inexistente");

        Assert.Equal("DENY", Assert.Single(response.Headers.GetValues("X-Frame-Options")));
        Assert.Equal("nosniff", Assert.Single(response.Headers.GetValues("X-Content-Type-Options")));
        Assert.Equal("no-referrer", Assert.Single(response.Headers.GetValues("Referrer-Policy")));
        Assert.Contains("frame-ancestors 'none'", Assert.Single(response.Headers.GetValues("Content-Security-Policy")));
        Assert.Equal(
            "camera=(), microphone=(), geolocation=(), payment=(), usb=()",
            Assert.Single(response.Headers.GetValues("Permissions-Policy")));
    }

    [Fact]
    public void Kestrel_NaoDeveDivulgarIdentificacaoDoServidor()
    {
        using var factory = new AtronPlatformProductionFactory();
        _ = factory.CreateClient();
        var kestrel = factory.Services.GetRequiredService<IOptions<KestrelServerOptions>>().Value;

        Assert.False(kestrel.AddServerHeader);
    }

    [Theory]
    [InlineData(typeof(LoginRequestDTO))]
    [InlineData(typeof(UsuarioRegistroRequest))]
    [InlineData(typeof(SolicitarRecuperacaoSenhaRequest))]
    [InlineData(typeof(ReenviarConfirmacaoEmailRequest))]
    [InlineData(typeof(RedefinirSenhaRequest))]
    [InlineData(typeof(ConfirmarEmailRequest))]
    [InlineData(typeof(SolicitarReativacaoRequest))]
    [InlineData(typeof(ReativarContaRequest))]
    public void ContratosPublicosDeAcesso_DevemRejeitarPropriedadeDesconhecida(Type tipo)
    {
        Assert.Throws<JsonException>(() =>
            JsonSerializer.Deserialize("{\"propriedadeInesperada\":true}", tipo));
    }
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
                    "Host=localhost;Database=atron_platform_tests;Username=tests;Password=tests",
                ["ProcessamentosProdutosLote:WorkerHabilitado"] = "false"
            });
        });
    }
}

public sealed class AtronPlatformProductionFactory : WebApplicationFactory<AtronPlatform.WebApi.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:SecretKey"] =
                    "chave-local-para-testes-do-host-neutro-123456789",
                ["ConnectionStrings:DefaultConnection"] =
                    "Host=localhost;Database=atron_platform_tests;Username=tests;Password=tests",
                ["ProcessamentosProdutosLote:WorkerHabilitado"] = "false"
            });
        });
    }
}
