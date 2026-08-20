using Application.Mapping;
using Application.Records.Tarefa;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class AtualizarTarefaMovimentacaoCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DevePreservarAtorEAlteracoesRelevantes()
    {
        TarefaMovimentacao? registrada = null;
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(item => registrada = item)
            .ReturnsAsync(true);
        var caseDeAtualizacao = new AtualizarTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());
        var responsavel = new Usuario
        {
            Codigo = "USR001",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
        var anterior = new Tarefa
        {
            Id = 42,
            Titulo = "Título anterior",
            Conteudo = "Conteúdo anterior",
            DataInicial = new DateTime(2026, 7, 1),
            DataFinal = new DateTime(2026, 7, 10),
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" }
        };
        var atual = new Tarefa
        {
            Id = 42,
            Titulo = "Título novo",
            Conteudo = "Conteúdo anterior",
            DataInicial = anterior.DataInicial,
            DataFinal = anterior.DataFinal,
            TarefaEstadoId = 5,
            EstadoDaTarefa = new TarefaEstado { Id = 5, Descricao = "Iniciada" }
        };

        var resultado = await caseDeAtualizacao.ExecutarAsync(
            new AtualizacaoMovimentacaoRecord(anterior, atual, responsavel));

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(registrada);
        Assert.Equal(TipoMovimentacaoTarefa.Atualizacao, registrada.Tipo);
        Assert.Equal("USR001", registrada.ResponsavelCodigo);
        Assert.Equal("Maria Silva", registrada.ResponsavelNome);
        Assert.Contains("Título atualizado.", registrada.Descricao);
        Assert.Contains("Estado alterado de Em atividade para Iniciada.", registrada.Descricao);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarFalhaTipadaQuandoPersistenciaFalhar()
    {
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);
        var caseDeAtualizacao = new AtualizarTarefaMovimentacaoCase(
            repository.Object,
            new TarefaMovimentacaoMapping());
        var parametros = new AtualizacaoMovimentacaoRecord(
            new Tarefa { Id = 42 },
            new Tarefa { Id = 42 },
            new Usuario
            {
                Codigo = "USR001",
                Nome = "Maria",
                Sobrenome = "Silva"
            });

        var resultado = await caseDeAtualizacao.ExecutarAsync(parametros);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
    }
}
