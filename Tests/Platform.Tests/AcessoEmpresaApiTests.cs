using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Application.DTO.Request;
using Application.DTO.Response;
using AtronPlatform.WebApi.Security;
using AtronTracker.Infrastructure.Context;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DTOS.Empresas;
using Xunit;

namespace Platform.Tests;

public sealed class AcessoEmpresaApiTests
{
    [Fact]
    public async Task BuscaESolicitacao_DevemCriarPedidoPendenteParaOUsuarioAutenticado()
    {
        using var factory = new EmpresaApiFactory();
        using var responsavel = await factory.CriarClienteAsync("RESPONSAVEL");
        using var ana = await factory.CriarClienteAsync("ANA");
        var cadastro = await responsavel.PostAsJsonAsync("/api/Empresa", EmpresaRequest());
        var empresa = await cadastro.Content.ReadFromJsonAsync<EmpresaResponse>();

        var busca = (await ana.GetFromJsonAsync<EmpresaBuscaResponse[]>(
            "/api/Empresa/Busca?termo=Empresa%20de%20estudos"))!;
        var empresaEncontrada = Assert.Single(busca!);
        Assert.Equal(empresa!.Id, empresaEncontrada.Id);
        Assert.Equal(empresa.Codigo, empresaEncontrada.Codigo);

        var solicitacao = await ana.PostAsJsonAsync("/api/Empresa/Solicitacoes?usuarioCodigo=RESPONSAVEL", new
        {
            empresaId = empresa.Id,
            usuarioId = 999
        });
        Assert.Equal(HttpStatusCode.Accepted, solicitacao.StatusCode);
        var dados = await solicitacao.Content.ReadFromJsonAsync<SolicitacaoEmpresaResponse>();
        Assert.Equal(empresa.Id, dados!.EmpresaId);
        Assert.Equal(StatusSolicitacaoEmpresa.Pendente, dados.Status);

        Assert.Equal(HttpStatusCode.BadRequest,
            (await ana.PostAsJsonAsync("/api/Empresa/Solicitacoes", new { empresaId = empresa.Id })).StatusCode);
        using var scope = factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
        Assert.Single(await context.SolicitacoesEmpresa.ToListAsync());
        Assert.Equal(1, await context.UsuariosEmpresas.CountAsync());
    }

    [Theory]
    [InlineData("GET", "/api/Modulo")]
    [InlineData("GET", "/api/Cargo")]
    [InlineData("POST", "/api/Cargo")]
    [InlineData("PUT", "/api/Cargo/TESTE")]
    [InlineData("DELETE", "/api/Cargo/TESTE")]
    [InlineData("GET", "/api/Usuario")]
    [InlineData("GET", "/api/PerfilDeAcesso")]
    [InlineData("GET", "/api/Fornecedor")]
    [InlineData("GET", "/api/Categoria")]
    [InlineData("GET", "/api/Produto")]
    [InlineData("GET", "/api/processamentos-produtos")]
    [InlineData("GET", "/api/notificacoes")]
    [InlineData("GET", "/api/notificacoes/saude")]
    [InlineData("GET", "/Auditoria/registro/contexto")]
    public async Task SemEmpresa_DeveBloquearRotasOperacionaisMesmoComPermissoes(string metodo, string rota)
    {
        using var factory = new EmpresaApiFactory();
        using var client = await factory.CriarClienteAsync("ANA");
        factory.DefinirModulos("ANA", "CRG", "USR", "PERF", "CAT", "PRD");
        using var request = new HttpRequestMessage(new HttpMethod(metodo), rota);
        request.Headers.Add("X-Empresa-Id", "999");
        request.Content = JsonContent.Create(new { empresaId = 999 });

        var response = await client.SendAsync(request);

        await AssertBloqueio(response);
    }

    [Fact]
    public async Task Contexto_DeveExigirLoginEManterSessaoDisponivelSemEmpresa()
    {
        using var factory = new EmpresaApiFactory();
        using var anonimo = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync("/api/Empresa/Contexto")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonimo.GetAsync("/api/Modulo")).StatusCode);

        using var client = await factory.CriarClienteAsync("ANA");
        var response = await client.GetAsync("/api/Empresa/Contexto");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.CacheControl!.NoStore);
        var contexto = await response.Content.ReadFromJsonAsync<ContextoEmpresa>();
        Assert.False(contexto!.AcessoPermitido);
        Assert.Null(contexto.EmpresaId);
        Assert.NotEmpty(contexto.MotivoBloqueio!);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/Sessao/Info")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.GetAsync("/api/Empresa")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/saude")).StatusCode);
    }

    [Fact]
    public async Task Cadastro_DeveLiberarEmpresaMasNaoConcederPermissoesDeModulo()
    {
        using var factory = new EmpresaApiFactory();
        using var client = await factory.CriarClienteAsync("ANA");
        await AssertBloqueio(await client.GetAsync("/api/Modulo"));

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
        var contexto = await client.GetFromJsonAsync<ContextoEmpresa>("/api/Empresa/Contexto");
        Assert.True(contexto!.AcessoPermitido);
        Assert.NotNull(contexto.EmpresaId);
        Assert.Equal("ESTUDO", contexto.Codigo);
        Assert.Null(contexto.MotivoBloqueio);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/Modulo")).StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync("/api/Cargo")).StatusCode);
        factory.DefinirModulos("ANA", "CRG");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/Cargo")).StatusCode);
    }

    [Theory]
    [InlineData(StatusEmpresa.Pendente, StatusUsuarioEmpresa.Ativo)]
    [InlineData(StatusEmpresa.Suspensa, StatusUsuarioEmpresa.Ativo)]
    [InlineData(StatusEmpresa.Ativa, StatusUsuarioEmpresa.Suspenso)]
    [InlineData(StatusEmpresa.Ativa, StatusUsuarioEmpresa.Encerrado)]
    public async Task MudancaDeStatus_DeveBloquearNaProximaRequisicaoSemRenovarSessao(
        StatusEmpresa statusEmpresa, StatusUsuarioEmpresa statusVinculo)
    {
        using var factory = new EmpresaApiFactory();
        using var client = await factory.CriarClienteAsync("ANA");
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
        factory.DefinirModulos("ANA", "CRG");
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/Cargo")).StatusCode);

        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
            var empresa = await db.Empresas.SingleAsync();
            var vinculo = await db.UsuariosEmpresas.SingleAsync();
            db.Entry(empresa).Property(e => e.Status).CurrentValue = statusEmpresa;
            db.Entry(vinculo).Property(v => v.Status).CurrentValue = statusVinculo;
            await db.SaveChangesAsync();
        }

        await AssertBloqueio(await client.GetAsync("/api/Cargo"));
        var contexto = await client.GetFromJsonAsync<ContextoEmpresa>("/api/Empresa/Contexto");
        Assert.False(contexto!.AcessoPermitido);
        Assert.NotNull(contexto.EmpresaId);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task ContaInativaOuEmailNaoConfirmado_DeveBloquearMesmoComEmpresa(bool inativo, bool confirmado)
    {
        using var factory = new EmpresaApiFactory();
        using var client = await factory.CriarClienteAsync("ANA");
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AtronDbContext>();
            var usuario = await db.Usuarios.SingleAsync();
            usuario.Inativo = inativo;
            usuario.EmailConfirmado = confirmado;
            await db.SaveChangesAsync();
        }
        await AssertBloqueio(await client.GetAsync("/api/Modulo"));
        Assert.False((await client.GetFromJsonAsync<ContextoEmpresa>("/api/Empresa/Contexto"))!.AcessoPermitido);
    }

    [Fact]
    public async Task Contexto_NaoDeveAceitarEmpresaOuUsuarioInformadosPeloCliente()
    {
        using var factory = new EmpresaApiFactory();
        using var ana = await factory.CriarClienteAsync("ANA");
        using var bruno = await factory.CriarClienteAsync("BRUNO");
        Assert.Equal(HttpStatusCode.Created, (await ana.PostAsJsonAsync("/api/Empresa", Request())).StatusCode);
        var empresaAna = await ana.GetFromJsonAsync<ContextoEmpresa>("/api/Empresa/Contexto");
        bruno.DefaultRequestHeaders.Add("X-Empresa-Id", empresaAna!.EmpresaId.ToString());
        var contexto = await bruno.GetFromJsonAsync<ContextoEmpresa>(
            $"/api/Empresa/Contexto?usuarioCodigo=ANA&empresaId={empresaAna.EmpresaId}");
        Assert.False(contexto!.AcessoPermitido);
        Assert.Null(contexto.EmpresaId);
        await AssertBloqueio(await bruno.GetAsync($"/api/Modulo?empresaId={empresaAna.EmpresaId}"));
    }

    [Fact]
    public async Task UsuarioInexistente_DeveSerBloqueado()
    {
        using var factory = new EmpresaApiFactory();
        using var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        client.DefaultRequestHeaders.Add("X-Usuario-Teste", "INEXISTE");
        await AssertBloqueio(await client.GetAsync("/api/Modulo"));
    }

    [Fact]
    public void ExcecoesSemEmpresa_DevemSerRestritasAoCadastroConsultaESessao()
    {
        using var factory = new EmpresaApiFactory();
        using var client = factory.CreateClient();
        var excecoes = factory.Services.GetRequiredService<EndpointDataSource>().Endpoints
            .Where(e => e.Metadata.GetMetadata<PermitirSemEmpresaAttribute>() is not null)
            .Select(e => e.Metadata.GetMetadata<ControllerActionDescriptor>()!)
            .Select(a => $"{a.ControllerName}/{a.ActionName}").Order().ToArray();
        Assert.Equal(new[] { "Acesso/Logout", "Empresa/Associacao", "Empresa/Busca", "Empresa/Contexto", "Empresa/Get", "Empresa/Post", "Empresa/Solicitar", "Sessao/SesssaoInfo" }, excecoes);
    }

    private static async Task AssertBloqueio(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("EMPRESA_ACESSO_BLOQUEADO", json.GetProperty("codigo").GetString());
        Assert.False(string.IsNullOrWhiteSpace(json.GetProperty("mensagem").GetString()));
    }

    private static EmpresaCadastroRequest EmpresaRequest() => new()
    {
        Codigo = "ESTUDO", NomeFantasia = "Empresa de estudos", Email = "empresa@example.test",
        Numero = "(11) 99999-0000", Endereco = new EnderecoEmpresaRequest { Logradouro = "Rua de Teste" }
    };

    private static EmpresaCadastroRequest Request() => new()
    {
        Codigo = "ESTUDO", NomeFantasia = "Empresa de estudos", Email = "empresa@example.test",
        Numero = "(11) 99999-0000", Endereco = new EnderecoEmpresaRequest { Logradouro = "Rua de Teste" }
    };
}
