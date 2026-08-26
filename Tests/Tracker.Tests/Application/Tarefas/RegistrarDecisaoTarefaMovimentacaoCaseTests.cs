using Application.Mapping;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class RegistrarDecisaoTarefaMovimentacaoCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveRegistrarAprovacaoComContextoDaDecisao()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var (caseDeMovimentacao, repository, solicitacao, responsavel) = CriarCenario();
        var inicio = DateTime.UtcNow;
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);

        var resultado = await caseDeMovimentacao.ExecutarAsync(
            solicitacao,
            responsavel,
            aprovar: true);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(solicitacao.TarefaId, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.AprovacaoObtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(responsavel.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal("Gestor Aprovador", movimentacaoRegistrada.ResponsavelNome);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheAprovacao,
                "Usuario Solicitante",
                "Iniciada"),
            movimentacaoRegistrada.Descricao);
        Assert.InRange(movimentacaoRegistrada.DataOcorrencia, inicio, DateTime.UtcNow);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRegistrarRecusaComContextoDaDecisao()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var (caseDeMovimentacao, repository, solicitacao, responsavel) = CriarCenario();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);

        var resultado = await caseDeMovimentacao.ExecutarAsync(
            solicitacao,
            responsavel,
            aprovar: false);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(solicitacao.TarefaId, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.RecusaObtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(responsavel.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheRecusa,
                "Usuario Solicitante"),
            movimentacaoRegistrada.Descricao);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarFalhaQuandoMovimentacaoNaoForPersistida()
    {
        var (caseDeMovimentacao, repository, solicitacao, responsavel) = CriarCenario();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);

        var resultado = await caseDeMovimentacao.ExecutarAsync(
            solicitacao,
            responsavel,
            aprovar: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
        repository.Verify(
            item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Once);
    }

    private static (
        RegistrarDecisaoTarefaMovimentacaoCase Case,
        Mock<ITarefaMovimentacaoRepository> Repository,
        SolicitacaoObtencaoTarefa Solicitacao,
        Usuario Responsavel) CriarCenario()
    {
        var solicitante = new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Solicitante"
        };
        var responsavel = new Usuario
        {
            Id = 20,
            Codigo = "GST",
            Nome = "Gestor",
            Sobrenome = "Aprovador"
        };
        var tarefa = new Tarefa
        {
            Id = 30,
            TarefaEstadoId = 5,
            EstadoDaTarefa = new TarefaEstado
            {
                Id = 5,
                Descricao = "Iniciada"
            }
        };
        var solicitacao = new SolicitacaoObtencaoTarefa
        {
            Id = 40,
            TarefaId = tarefa.Id,
            Tarefa = tarefa,
            SolicitanteId = solicitante.Id,
            SolicitanteCodigo = solicitante.Codigo,
            Solicitante = solicitante,
            AprovadorId = responsavel.Id,
            AprovadorCodigo = responsavel.Codigo,
            Aprovador = responsavel
        };
        var repository = new Mock<ITarefaMovimentacaoRepository>();

        return (
            new RegistrarDecisaoTarefaMovimentacaoCase(
                repository.Object,
                new TarefaMovimentacaoMapping()),
            repository,
            solicitacao,
            responsavel);
    }
}
