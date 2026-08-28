using Application.UseCases.EmpresaCases;
using Application.Services.EntitiesServices.Empresas;
using Application.Validador;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shared.Application.Interfaces.Service;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class DecisaoSolicitacaoEmpresaTests
{
    [Fact]
    public async Task Aprovar_DeveCriarVinculoMembroEAtualizarSolicitacao()
    {
        await using var contexto = await PrepararAsync();
        var solicitacao = await contexto.SolicitacoesEmpresa.SingleAsync();
        var (caso, notificacoes) = CriarCaso(contexto, "RESPONSAVEL");

        var resultado = await caso.AprovarAsync(solicitacao.Id);

        Assert.True(resultado.TeveSucesso);
        var vinculo = await contexto.UsuariosEmpresas.SingleAsync(item => item.UsuarioCodigo == "ANA");
        Assert.Equal(PapelUsuarioEmpresa.Membro, vinculo.Papel);
        Assert.Equal(StatusUsuarioEmpresa.Ativo, vinculo.Status);
        Assert.Equal(StatusSolicitacaoEmpresa.Aprovada, (await contexto.SolicitacoesEmpresa.SingleAsync()).Status);
        Assert.Equal("ANA", notificacoes.Single().DestinatarioCodigo);
        Assert.Equal("Empresa.AssociacaoAprovada", notificacoes.Single().TipoEvento);
    }

    [Fact]
    public async Task Recusar_DevePreservarSolicitacaoSemCriarVinculo()
    {
        await using var contexto = await PrepararAsync();
        var solicitacao = await contexto.SolicitacoesEmpresa.SingleAsync();
        var (caso, notificacoes) = CriarCaso(contexto, "RESPONSAVEL");

        var resultado = await caso.RecusarAsync(solicitacao.Id);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(StatusSolicitacaoEmpresa.Recusada, (await contexto.SolicitacoesEmpresa.SingleAsync()).Status);
        Assert.DoesNotContain(await contexto.UsuariosEmpresas.ToListAsync(), item => item.UsuarioCodigo == "ANA");
        Assert.Equal("Empresa.AssociacaoRecusada", notificacoes.Single().TipoEvento);
    }

    [Fact]
    public async Task MembroNaoPodeDecidirSolicitacoesDaEmpresa()
    {
        await using var contexto = await PrepararAsync();
        var solicitacao = await contexto.SolicitacoesEmpresa.SingleAsync();
        var (caso, notificacoes) = CriarCaso(contexto, "ANA");

        var resultado = await caso.AprovarAsync(solicitacao.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Empty(notificacoes);
        Assert.Equal(StatusSolicitacaoEmpresa.Pendente, (await contexto.SolicitacoesEmpresa.SingleAsync()).Status);
    }

    private static (DecidirSolicitacaoEmpresaCase Caso, List<PublicarNotificacaoInternaRequest> Notificacoes)
        CriarCaso(AtronDbContext contexto, string codigo)
    {
        var repository = new EmpresaRepository(contexto);
        var accessor = new Mock<IUserAccessor>();
        accessor.Setup(item => item.ObterCodigoUsuarioLogado()).Returns(codigo);
        var usuarioAtual = new UsuarioEmpresaAtualService(accessor.Object, repository, new EmpresaCadastroValidador());
        var responsavel = new EmpresaResponsavelService(usuarioAtual, repository);
        var notificacoes = new List<PublicarNotificacaoInternaRequest>();
        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher.Setup(item => item.PublicarAsync(It.IsAny<PublicarNotificacaoInternaRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PublicarNotificacaoInternaRequest, CancellationToken>((request, _) => notificacoes.Add(request))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Falha("teste"));
        return (new DecidirSolicitacaoEmpresaCase(responsavel, repository, publisher.Object), notificacoes);
    }

    private static async Task<AtronDbContext> PrepararAsync()
    {
        var contexto = new AtronDbContext(new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        var responsavel = new Usuario("RESPONSAVEL", "Resp", "Teste", "resp@example.test", null) { EmailConfirmado = true };
        var ana = new Usuario("ANA", "Ana", "Teste", "ana@example.test", null) { EmailConfirmado = true };
        contexto.Usuarios.AddRange(responsavel, ana);
        await contexto.SaveChangesAsync();
        var empresa = new Empresa
        {
            Codigo = "ATR-EMP", NomeFantasia = "Empresa", Email = "empresa@example.test",
            Numero = "(11) 99999-0000", Endereco = new Domain.ValueObjects.Endereco { Logradouro = "Rua" }
        };
        empresa.ConcluirCadastro(responsavel);
        contexto.Empresas.Add(empresa);
        await contexto.SaveChangesAsync();
        contexto.ChangeTracker.Clear();
        var solicitacao = empresa.CriarSolicitacao(ana);
        contexto.Entry(empresa).State = EntityState.Unchanged;
        contexto.Entry(ana).State = EntityState.Unchanged;
        contexto.SolicitacoesEmpresa.Add(solicitacao);
        await contexto.SaveChangesAsync();
        contexto.ChangeTracker.Clear();
        return contexto;
    }
}
