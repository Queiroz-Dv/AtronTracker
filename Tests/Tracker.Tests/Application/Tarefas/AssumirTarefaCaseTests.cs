using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Policies.Tarefas;
using Application.Resources;
using Application.UseCases.TarefaCases;
using Application.UseCases.TarefaCases.Movimentacao;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class AssumirTarefaCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveBloquearUsuarioSemResponsabilidadeDeGestao()
    {
        var cenario = CriarCenario();
        cenario.Tarefas
            .Setup(repository => repository.PossuiResponsabilidadeGestaoAsync(
                cenario.Usuario.Id,
                cenario.Usuario.Codigo))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.TarefaAnterior.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaExigeSolicitacaoObtencao);
        cenario.Tarefas.Verify(
            repository => repository.AssumirTarefaAsync(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DeveAssumirRegistrarMovimentacaoEPreservarEstadoOperacional()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);
        var estadoAntesDaObtencao = cenario.TarefaAnterior.TarefaEstadoId;
        var inicio = DateTime.UtcNow;

        var resultado = await cenario.Case.ExecutarAsync(cenario.TarefaAnterior.Id);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(estadoAntesDaObtencao, cenario.TarefaAtualizada.TarefaEstadoId);
        Assert.Equal(estadoAntesDaObtencao, resultado.Dados!.EstadoDaTarefa.Id);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(cenario.TarefaAnterior.Id, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Obtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(cenario.Usuario.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal("Maria Silva", movimentacaoRegistrada.ResponsavelNome);
        Assert.Equal(
            string.Format(TarefaResource.Historico_DetalheObtencao, "Maria Silva"),
            movimentacaoRegistrada.Descricao);
        Assert.InRange(movimentacaoRegistrada.DataOcorrencia, inicio, DateTime.UtcNow);
        cenario.Tarefas.Verify(
            repository => repository.AtualizarTarefaAsync(
                It.IsAny<int>(),
                It.IsAny<Tarefa>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarFalhaQuandoAssuncaoNaoForPersistida()
    {
        var cenario = CriarCenario();
        cenario.Tarefas
            .Setup(repository => repository.AssumirTarefaAsync(
                cenario.TarefaAnterior.Id,
                cenario.Usuario.Id,
                cenario.Usuario.Codigo))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.TarefaAnterior.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_AssumirTarefa);
        cenario.Movimentacoes.Verify(
            repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Never);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarFalhaDaMovimentacaoSemPublicarNotificacao()
    {
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.TarefaAnterior.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CenarioAssuncao CriarCenario()
    {
        var usuario = new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
        var tarefaAnterior = CriarTarefa();
        var tarefaAtualizada = CriarTarefa();
        tarefaAtualizada.UsuarioId = usuario.Id;
        tarefaAtualizada.UsuarioCodigo = usuario.Codigo;
        tarefaAtualizada.Usuario = usuario;
        var assumida = false;

        var tarefas = new Mock<ITarefaRepository>();
        tarefas
            .Setup(repository => repository.ObterTarefaPorId(tarefaAnterior.Id))
            .ReturnsAsync(() => assumida ? tarefaAtualizada : tarefaAnterior);
        tarefas
            .Setup(repository => repository.PossuiResponsabilidadeGestaoAsync(
                usuario.Id,
                usuario.Codigo))
            .ReturnsAsync(true);
        tarefas
            .Setup(repository => repository.AssumirTarefaAsync(
                tarefaAnterior.Id,
                usuario.Id,
                usuario.Codigo))
            .Callback(() => assumida = true)
            .ReturnsAsync(true);

        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var movimentacoes = new Mock<ITarefaMovimentacaoRepository>();
        movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(true);

        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher
            .Setup(service => service.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Sucesso(
                new NotificacaoInternaResponse(
                    1000001,
                    "Tracker",
                    "TarefaObtida",
                    "",
                    "",
                    null,
                    null,
                    false,
                    DateTimeOffset.UtcNow,
                    null)));

        var mapper = new Mock<IToDtoMapper<Tarefa, TarefaDTO>>();
        mapper
            .Setup(item => item.MapToDto(tarefaAtualizada))
            .Returns(new TarefaDTO
            {
                Id = tarefaAtualizada.Id,
                UsuarioCodigo = usuario.Codigo,
                EstadoDaTarefa = new TarefaEstadoDTO
                {
                    Id = tarefaAtualizada.TarefaEstadoId,
                    Descricao = tarefaAtualizada.EstadoDaTarefa.Descricao
                }
            });

        var caseDeAssuncao = new AssumirTarefaCase(
            tarefas.Object,
            usuarioService.Object,
            new TarefaObtencaoPolicy(),
            new TarefaNotificacaoInternaCase(publisher.Object),
            mapper.Object,
            new RegistrarObtencaoTarefaMovimentacaoCase(
                movimentacoes.Object,
                new TarefaMovimentacaoMapping()));

        return new CenarioAssuncao(
            caseDeAssuncao,
            tarefas,
            movimentacoes,
            publisher,
            usuario,
            tarefaAnterior,
            tarefaAtualizada);
    }

    private static Tarefa CriarTarefa()
    {
        return new Tarefa
        {
            Id = 30,
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" },
            ExigeAprovacaoParaObter = false
        };
    }

    private sealed record CenarioAssuncao(
        AssumirTarefaCase Case,
        Mock<ITarefaRepository> Tarefas,
        Mock<ITarefaMovimentacaoRepository> Movimentacoes,
        Mock<INotificacoesInternasPublisher> Publisher,
        Usuario Usuario,
        Tarefa TarefaAnterior,
        Tarefa TarefaAtualizada);
}
