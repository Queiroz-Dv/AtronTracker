using Application.Mapping;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class RegistrarObtencaoTarefaMovimentacaoCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveRegistrarFotografiaDaObtencaoSemAlterarEstado()
    {
        var tarefa = CriarTarefa();
        var responsavel = CriarResponsavel();
        var estadoAnterior = tarefa.TarefaEstadoId;
        var inicio = DateTime.UtcNow;
        TarefaMovimentacao? registrada = null;
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(item => registrada = item)
            .ReturnsAsync(true);
        var caseDeMovimentacao = new RegistrarObtencaoTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeMovimentacao.ExecutarAsync(tarefa, responsavel);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(estadoAnterior, tarefa.TarefaEstadoId);
        Assert.NotNull(registrada);
        Assert.Equal(tarefa.Id, registrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Obtencao, registrada.Tipo);
        Assert.Equal(responsavel.Codigo, registrada.ResponsavelCodigo);
        Assert.Equal("Maria Silva", registrada.ResponsavelNome);
        Assert.Equal(
            string.Format(TarefaResource.Historico_DetalheObtencao, "Maria Silva"),
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
        var caseDeMovimentacao = new RegistrarObtencaoTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());

        var resultado = await caseDeMovimentacao.ExecutarAsync(
            CriarTarefa(),
            CriarResponsavel());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
    }

    private static Tarefa CriarTarefa()
    {
        return new Tarefa
        {
            Id = 30,
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" }
        };
    }

    private static Usuario CriarResponsavel()
    {
        return new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
    }
}
