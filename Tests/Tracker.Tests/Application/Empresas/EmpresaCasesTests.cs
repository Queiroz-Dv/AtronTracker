using Application.Mapping;
using Application.Resources;
using Application.Services.EntitiesServices.Empresas;
using Application.UseCases.EmpresaCases;
using Application.Validador;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Application.Interfaces.Service;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EmpresaCasesTests
{
    [Fact]
    public async Task Cadastrar_DeveGravarEmpresaEVinculoDoUsuarioDaSessaoSemDuplicarUsuario()
    {
        await using var contexto = CriarContexto();
        var usuario = await CadastrarUsuario(contexto, "ANA");
        var (cadastrar, consultar) = CriarCasos(new EmpresaRepository(contexto), "ANA");
        var request = EmpresaCadastroValidadorTests.RequestValido();

        var resultado = await cadastrar.ExecutarAsync(request);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(request.Codigo, resultado.Dados!.Codigo);
        Assert.Equal(PapelUsuarioEmpresa.Responsavel, resultado.Dados.PapelUsuario);
        Assert.Equal(StatusEmpresa.Ativa, resultado.Dados.Status);
        var vinculo = await contexto.UsuariosEmpresas.SingleAsync();
        Assert.Equal(usuario.Id, vinculo.UsuarioId);
        Assert.Equal("ANA", vinculo.UsuarioCodigo);
        Assert.Equal(1, await contexto.Usuarios.CountAsync());
        Assert.Equal(1, await contexto.Empresas.CountAsync());
        Assert.Equal(resultado.Dados, (await consultar.ExecutarAsync()).Dados);
    }

    [Theory]
    [InlineData(true, true)]
    [InlineData(false, false)]
    public async Task Cadastrar_DeveRecusarContaInativaOuNaoConfirmada(bool inativo, bool confirmado)
    {
        await using var contexto = CriarContexto();
        var usuario = await CadastrarUsuario(contexto, "ANA");
        usuario.Inativo = inativo;
        usuario.EmailConfirmado = confirmado;
        await contexto.SaveChangesAsync();
        var (cadastrar, consultar) = CriarCasos(new EmpresaRepository(contexto), "ANA");

        Assert.True((await cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido())).TeveFalha);
        Assert.True((await consultar.ExecutarAsync()).TeveFalha);
        Assert.Empty(await contexto.Empresas.ToListAsync());
        Assert.Empty(await contexto.UsuariosEmpresas.ToListAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("NAO_EXISTE")]
    public async Task Cadastrar_DeveRecusarSessaoSemUsuarioValido(string codigo)
    {
        await using var contexto = CriarContexto();
        var (cadastrar, consultar) = CriarCasos(new EmpresaRepository(contexto), codigo);

        Assert.True((await cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido())).TeveFalha);
        Assert.True((await consultar.ExecutarAsync()).TeveFalha);
        Assert.Empty(await contexto.Empresas.ToListAsync());
    }

    [Fact]
    public async Task Cadastrar_DeveRecusarSegundaEmpresaSemSubstituirVinculo()
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "ANA");
        var (cadastrar, _) = CriarCasos(new EmpresaRepository(contexto), "ANA");
        var primeira = await cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido());
        var outra = EmpresaCadastroValidadorTests.RequestValido();
        outra.Codigo = "Outra";

        var resultado = await cadastrar.ExecutarAsync(outra);

        Assert.Equal(EmpresaResource.Erro_UsuarioJaVinculado, Assert.Single(resultado.Messages).Descricao);
        Assert.Equal(primeira.Dados!.Id, (await contexto.UsuariosEmpresas.SingleAsync()).EmpresaId);
        Assert.Equal(1, await contexto.Empresas.CountAsync());
    }

    [Fact]
    public async Task Cadastrar_DeveRecusarCodigoExistenteSemVincularOutroUsuario()
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "ANA");
        await CadastrarUsuario(contexto, "BRUNO");
        var repository = new EmpresaRepository(contexto);
        var (primeiro, _) = CriarCasos(repository, "ANA");
        await primeiro.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido());
        var (segundo, _) = CriarCasos(repository, "BRUNO");

        var resultado = await segundo.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido());

        Assert.Equal(EmpresaResource.Erro_CodigoExistente, Assert.Single(resultado.Messages).Descricao);
        Assert.Equal("ANA", (await contexto.UsuariosEmpresas.SingleAsync()).UsuarioCodigo);
    }

    [Fact]
    public async Task Consultar_DeveRetornarApenasAEmpresaDoUsuarioConectado()
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "ANA");
        await CadastrarUsuario(contexto, "BRUNO");
        await CadastrarUsuario(contexto, "CARLA");
        var repository = new EmpresaRepository(contexto);
        var (cadastrarAna, consultarAna) = CriarCasos(repository, "ANA");
        var (cadastrarBruno, consultarBruno) = CriarCasos(repository, "BRUNO");
        await cadastrarAna.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido());
        var request = EmpresaCadastroValidadorTests.RequestValido();
        request.Codigo = "EmpresaBruno";
        await cadastrarBruno.ExecutarAsync(request);

        Assert.Equal("Estudo", (await consultarAna.ExecutarAsync()).Dados!.Codigo);
        Assert.Equal("EmpresaBruno", (await consultarBruno.ExecutarAsync()).Dados!.Codigo);
        var (_, consultarCarla) = CriarCasos(repository, "CARLA");
        var semVinculo = await consultarCarla.ExecutarAsync();
        Assert.True(semVinculo.TeveSucesso);
        Assert.Null(semVinculo.Dados);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Consultar_DeveRecusarEmpresaOuVinculoSuspenso(bool suspenderEmpresa)
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "ANA");
        var (cadastrar, consultar) = CriarCasos(new EmpresaRepository(contexto), "ANA");
        await cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido());
        var vinculo = await contexto.UsuariosEmpresas.Include(item => item.Empresa).SingleAsync();
        if (suspenderEmpresa)
            contexto.Entry(vinculo.Empresa).Property(item => item.Status).CurrentValue = StatusEmpresa.Suspensa;
        else
            contexto.Entry(vinculo).Property(item => item.Status).CurrentValue = StatusUsuarioEmpresa.Suspenso;
        await contexto.SaveChangesAsync();

        var resultado = await consultar.ExecutarAsync();

        Assert.True(resultado.TeveFalha);
        Assert.Null(resultado.Dados);
        Assert.True((await cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido())).TeveFalha);
    }

    [Fact]
    public async Task Cadastrar_DevePropagarFalhaDePersistencia()
    {
        var repository = new Mock<IEmpresaRepository>();
        repository.Setup(item => item.ObterUsuarioAsync("ANA"))
            .ReturnsAsync(new Usuario { Id = 1, Codigo = "ANA", EmailConfirmado = true });
        var erro = new DbUpdateException("Falha de persistência simulada.");
        repository.Setup(item => item.CriarAsync(It.IsAny<Empresa>())).ThrowsAsync(erro);
        var (cadastrar, _) = CriarCasos(repository.Object, "ANA");

        var falha = await Assert.ThrowsAsync<DbUpdateException>(
            () => cadastrar.ExecutarAsync(EmpresaCadastroValidadorTests.RequestValido()));

        Assert.Same(erro, falha);
    }

    private static (CadastrarEmpresaCase, ObterEmpresaCase) CriarCasos(IEmpresaRepository repository, string codigo)
    {
        var accessor = new Mock<IUserAccessor>();
        accessor.Setup(item => item.ObterCodigoUsuarioLogado()).Returns(codigo);
        var validador = new EmpresaCadastroValidador();
        var usuarioAtual = new UsuarioEmpresaAtualService(accessor.Object, repository, validador);
        var mapping = new EmpresaMapping();
        return (new CadastrarEmpresaCase(usuarioAtual, validador, mapping, repository),
            new ObterEmpresaCase(usuarioAtual, mapping, repository));
    }

    private static AtronDbContext CriarContexto() => new(new DbContextOptionsBuilder<AtronDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<Usuario> CadastrarUsuario(AtronDbContext context, string codigo)
    {
        var usuario = new Usuario(codigo, codigo, "Teste", $"{codigo}@example.test", null) { EmailConfirmado = true };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        return usuario;
    }
}
