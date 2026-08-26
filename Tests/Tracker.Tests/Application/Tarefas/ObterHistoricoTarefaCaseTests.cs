using Application.Interfaces.Services;
using Application.Mapping;
using Application.Resources;
using Application.UseCases.TarefaCases;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class ObterHistoricoTarefaCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveConsultarColecaoCompletaEMapearMovimento()
    {
        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        movimentacaoRepository
            .Setup(item => item.ObterMovimentacoesPorIdAsync(42))
            .ReturnsAsync(
            [
                new TarefaMovimentacao
                {
                    Id = 10,
                    TarefaId = 42,
                    Tipo = TipoMovimentacaoTarefa.Criacao,
                    Descricao = "Detalhes",
                    ResponsavelCodigo = "USR001",
                    ResponsavelNome = "Maria Silva",
                    DataOcorrencia = new DateTime(2026, 7, 26, 12, 0, 0)
                }
            ]);
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository
            .Setup(item => item.PodeAcessarHistoricoAsync(42, 7, "USR001"))
            .ReturnsAsync(true);
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(item => item.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(
                new Usuario { Id = 7, Codigo = "USR001" }));
        var caseDeHistorico = new ObterHistoricoTarefaCase(
            usuarioService.Object,
            tarefaRepository.Object,
            movimentacaoRepository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeHistorico.ExecutarAsync(42);

        Assert.True(resultado.TeveSucesso);
        var movimentacao = Assert.Single(resultado.Dados);
        Assert.Equal(42, movimentacao.TarefaId);
        Assert.Equal("Criação", movimentacao.Movimento);
        movimentacaoRepository.Verify(
            item => item.ObterMovimentacoesPorIdAsync(42),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarColecaoVaziaParaUsuarioAutorizadoSemMovimentacoes()
    {
        var (caseDeHistorico, tarefaRepository, movimentacaoRepository) =
            CriarCenarioAutorizado([]);

        var resultado = await caseDeHistorico.ExecutarAsync(42);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Empty(resultado.Dados);
        tarefaRepository.Verify(
            item => item.PodeAcessarHistoricoAsync(42, 7, "USR001"),
            Times.Once);
        movimentacaoRepository.Verify(
            item => item.ObterMovimentacoesPorIdAsync(42),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveNegarAcessoSemConsultarMovimentacoes()
    {
        var (caseDeHistorico, tarefaRepository, movimentacaoRepository) =
            CriarCenarioAutorizado([]);
        tarefaRepository
            .Setup(item => item.PodeAcessarHistoricoAsync(42, 7, "USR001"))
            .ReturnsAsync(false);

        var resultado = await caseDeHistorico.ExecutarAsync(42);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_AcessoHistoricoNaoPermitido);
        movimentacaoRepository.Verify(
            item => item.ObterMovimentacoesPorIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarFalhaAoObterUsuarioSemConsultarAutorizacao()
    {
        const string mensagemDeFalha = "Não foi possível obter o usuário atual.";
        var tarefaRepository = new Mock<ITarefaRepository>();
        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(item => item.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Falha(mensagemDeFalha));
        var caseDeHistorico = new ObterHistoricoTarefaCase(
            usuarioService.Object,
            tarefaRepository.Object,
            movimentacaoRepository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeHistorico.ExecutarAsync(42);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == mensagemDeFalha);
        tarefaRepository.Verify(
            item => item.PodeAcessarHistoricoAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);
        movimentacaoRepository.Verify(
            item => item.ObterMovimentacoesPorIdAsync(It.IsAny<int>()),
            Times.Never);
    }

    private static (
        ObterHistoricoTarefaCase Case,
        Mock<ITarefaRepository> TarefaRepository,
        Mock<ITarefaMovimentacaoRepository> MovimentacaoRepository)
        CriarCenarioAutorizado(List<TarefaMovimentacao> movimentacoes)
    {
        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        movimentacaoRepository
            .Setup(item => item.ObterMovimentacoesPorIdAsync(42))
            .ReturnsAsync(movimentacoes);
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository
            .Setup(item => item.PodeAcessarHistoricoAsync(42, 7, "USR001"))
            .ReturnsAsync(true);
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(item => item.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(
                new Usuario { Id = 7, Codigo = "USR001" }));

        return (
            new ObterHistoricoTarefaCase(
                usuarioService.Object,
                tarefaRepository.Object,
                movimentacaoRepository.Object,
                new TarefaMovimentacaoMapping()),
            tarefaRepository,
            movimentacaoRepository);
    }
}
