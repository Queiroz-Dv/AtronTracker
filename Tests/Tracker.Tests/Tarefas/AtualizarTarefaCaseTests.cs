using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.UseCases.TarefaCases;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class AtualizarTarefaCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DevePersistirAtualizacaoEMovimentacaoComResponsavel()
    {
        var tarefaAnterior = CriarTarefa("Título anterior");
        var tarefaAtual = CriarTarefa("Título atualizado");
        var tarefaDto = new TarefaDTO { Titulo = tarefaAtual.Titulo };
        var responsavel = new Usuario
        {
            Id = 7,
            Codigo = "USR001",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
        TarefaMovimentacao? movimentacaoRegistrada = null;

        var tarefas = new Mock<ITarefaRepository>();
        tarefas
            .Setup(repository => repository.ObterTarefaPorId(tarefaAnterior.Id))
            .ReturnsAsync(tarefaAnterior);
        tarefas
            .Setup(repository => repository.AtualizarTarefaAsync(tarefaAnterior.Id, tarefaAtual))
            .ReturnsAsync(true);
        var preparacao = new Mock<ITarefaPreparacaoService>();
        preparacao
            .Setup(service => service.PrepararParaPersistenciaAsync(tarefaDto))
            .ReturnsAsync(Resultado<Tarefa>.Sucesso(tarefaAtual));
        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(responsavel));
        var movimentacoes = new Mock<ITarefaMovimentacaoRepository>();
        movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);
        var caseDeAtualizacao = new AtualizarTarefaCase(
            tarefas.Object,
            preparacao.Object,
            usuarioService.Object,
            new AtualizarTarefaMovimentacaoCase(
                movimentacoes.Object,
                new TarefaMovimentacaoMapping()));

        var resultado = await caseDeAtualizacao.ExecutarAsync(tarefaAnterior.Id, tarefaDto);

        Assert.True(resultado.TeveSucesso);
        Assert.Same(tarefaDto, resultado.Dados);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(tarefaAtual.Id, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Atualizacao, movimentacaoRegistrada.Tipo);
        Assert.Equal(responsavel.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal("Maria Silva", movimentacaoRegistrada.ResponsavelNome);
        Assert.Contains("Título atualizado.", movimentacaoRegistrada.Descricao);
        tarefas.Verify(
            repository => repository.AtualizarTarefaAsync(tarefaAnterior.Id, tarefaAtual),
            Times.Once);
    }

    private static Tarefa CriarTarefa(string titulo)
    {
        return new Tarefa
        {
            Id = 42,
            Titulo = titulo,
            Conteudo = "Conteúdo",
            DataInicial = new DateTime(2026, 8, 18),
            DataFinal = new DateTime(2026, 8, 20),
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" }
        };
    }
}
