using Application.DTO.Request;
using Application.Services.EntitiesServices.Empresas;
using Application.UseCases.EmpresaCases;
using Application.Validador;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Extensions;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Application.Interfaces.Service;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class AssociacaoEmpresaTests
{
    [Fact]
    public async Task Buscar_DeveRetornarSomenteEmpresasAtivasEComDadosPublicos()
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "RESPONSAVEL");
        await CadastrarUsuario(contexto, "ANA");
        var empresa = new Empresa
        {
            Codigo = "ATR-EMP", NomeFantasia = "Empresa aberta", Email = "empresa@example.test",
            Numero = "(11) 99999-0000", Endereco = new Domain.ValueObjects.Endereco { Logradouro = "Rua privada" }
        };
        empresa.ConcluirCadastro(await contexto.Usuarios.SingleAsync(usuario => usuario.Codigo == "RESPONSAVEL"));
        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();

        var resultado = await CriarBusca(contexto, "ANA").ExecutarAsync("ATR");

        var item = Assert.Single(resultado.Dados!);
        Assert.Equal("ATR-EMP", item.Codigo);
        Assert.Equal("Empresa aberta", item.NomeFantasia);
        Assert.DoesNotContain("Rua", item.NomeFantasia);
    }

    [Fact]
    public async Task Buscar_NaoDeveRetornarEmpresaPendente()
    {
        await using var contexto = CriarContexto();
        await CadastrarUsuario(contexto, "ANA");
        contexto.Empresas.Add(new Empresa
        {
            Codigo = "PENDENTE", NomeFantasia = "Ainda não ativa", Email = "pendente@example.test",
            Numero = "(11) 99999-0000", Endereco = new Domain.ValueObjects.Endereco { Logradouro = "Rua" }
        });
        await contexto.SaveChangesAsync();

        var resultado = await CriarBusca(contexto, "ANA").ExecutarAsync(null);

        Assert.Empty(resultado.Dados!);
    }

    [Fact]
    public async Task Solicitar_DeveCriarPedidoPendenteSemCriarVinculo()
    {
        await using var contexto = CriarContexto();
        var responsavel = await CadastrarUsuario(contexto, "RESPONSAVEL");
        await CadastrarUsuario(contexto, "ANA");
        var empresa = new Empresa
        {
            Codigo = "ATR-EMP", NomeFantasia = "Empresa", Email = "empresa@example.test",
            Numero = "(11) 99999-0000", Endereco = new Domain.ValueObjects.Endereco { Logradouro = "Rua" }
        };
        empresa.ConcluirCadastro(responsavel);
        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();
        contexto.ChangeTracker.Clear();
        var caso = CriarSolicitacao(contexto, "ANA");

        var resultado = await caso.ExecutarAsync(new SolicitarAssociacaoEmpresaRequest(empresa.Id));

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(StatusSolicitacaoEmpresa.Pendente, resultado.Dados!.Status);
        Assert.Equal("ATR-EMP", resultado.Dados.CodigoEmpresa);
        Assert.Empty(await contexto.UsuariosEmpresas.Where(vinculo => vinculo.UsuarioCodigo == "ANA").ToListAsync());
        Assert.Single(await contexto.SolicitacoesEmpresa.ToListAsync());
    }

    [Fact]
    public async Task Solicitar_DeveRecusarPedidoPendenteDuplicado()
    {
        await using var contexto = CriarContexto();
        var responsavel = await CadastrarUsuario(contexto, "RESPONSAVEL");
        await CadastrarUsuario(contexto, "ANA");
        var empresa = new Empresa
        {
            Codigo = "ATR-EMP", NomeFantasia = "Empresa", Email = "empresa@example.test",
            Numero = "(11) 99999-0000", Endereco = new Domain.ValueObjects.Endereco { Logradouro = "Rua" }
        };
        empresa.ConcluirCadastro(responsavel);
        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();
        contexto.ChangeTracker.Clear();
        var caso = CriarSolicitacao(contexto, "ANA");
        var request = new SolicitarAssociacaoEmpresaRequest(empresa.Id);
        await caso.ExecutarAsync(request);

        var resultado = await caso.ExecutarAsync(request);

        Assert.True(resultado.TeveFalha);
        Assert.Equal("Já existe uma solicitação pendente para esta empresa.", Assert.Single(resultado.Messages).Descricao);
        Assert.Single(await contexto.SolicitacoesEmpresa.ToListAsync());
    }

    private static BuscarEmpresasCase CriarBusca(AtronDbContext contexto, string codigo)
    {
        var repository = new EmpresaRepository(contexto);
        var accessor = new Mock<IUserAccessor>();
        accessor.Setup(item => item.ObterCodigoUsuarioLogado()).Returns(codigo);
        return new BuscarEmpresasCase(new UsuarioEmpresaAtualService(accessor.Object, repository, new EmpresaCadastroValidador()), repository);
    }

    private static SolicitarAssociacaoEmpresaCase CriarSolicitacao(AtronDbContext contexto, string codigo)
    {
        var repository = new EmpresaRepository(contexto);
        var accessor = new Mock<IUserAccessor>();
        accessor.Setup(item => item.ObterCodigoUsuarioLogado()).Returns(codigo);
        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher.Setup(item => item.PublicarAsync(
                It.IsAny<AtronNotificacoes.Contracts.DTO.Request.PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Falha("notificação simulada"));
        return new SolicitarAssociacaoEmpresaCase(
            new UsuarioEmpresaAtualService(accessor.Object, repository, new EmpresaCadastroValidador()),
            repository, publisher.Object);
    }

    private static AtronDbContext CriarContexto() => new(new DbContextOptionsBuilder<AtronDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

    private static async Task<Usuario> CadastrarUsuario(AtronDbContext contexto, string codigo)
    {
        var usuario = new Usuario(codigo, codigo, "Teste", $"{codigo}@example.test", null) { EmailConfirmado = true };
        contexto.Usuarios.Add(usuario);
        await contexto.SaveChangesAsync();
        return usuario;
    }
}
