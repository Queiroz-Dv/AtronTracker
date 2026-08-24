using Application.Mapping;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class RegistrarSolicitacaoTarefaMovimentacaoCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveRegistrarFotografiaDaSolicitacaoPendente()
    {
        var solicitacao = CriarSolicitacao();
        var responsavel = CriarResponsavel();
        var inicio = DateTime.UtcNow;
        TarefaMovimentacao? registrada = null;
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(item => registrada = item)
            .ReturnsAsync(true);
        var caseDeMovimentacao = new RegistrarSolicitacaoTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeMovimentacao.ExecutarAsync(solicitacao, responsavel);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal((int)StatusSolicitacaoObtencaoTarefa.Pendente, solicitacao.Status);
        Assert.NotNull(registrada);
        Assert.Equal(solicitacao.TarefaId, registrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.SolicitacaoObtencao, registrada.Tipo);
        Assert.Equal(responsavel.Codigo, registrada.ResponsavelCodigo);
        Assert.Equal("Usuario Solicitante", registrada.ResponsavelNome);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheSolicitacao,
                "Gestor Aprovador"),
            registrada.Descricao);
        Assert.InRange(registrada.DataOcorrencia, inicio, DateTime.UtcNow);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarFalhaTipadaQuandoPersistenciaFalhar()
    {
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);
        var caseDeMovimentacao = new RegistrarSolicitacaoTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeMovimentacao.ExecutarAsync(
            CriarSolicitacao(),
            CriarResponsavel());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
    }

    private static SolicitacaoObtencaoTarefa CriarSolicitacao()
    {
        return new SolicitacaoObtencaoTarefa
        {
            TarefaId = 30,
            Status = (int)StatusSolicitacaoObtencaoTarefa.Pendente,
            Aprovador = new Usuario
            {
                Id = 20,
                Codigo = "GST",
                Nome = "Gestor",
                Sobrenome = "Aprovador"
            }
        };
    }

    private static Usuario CriarResponsavel()
    {
        return new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Solicitante"
        };
    }
}
