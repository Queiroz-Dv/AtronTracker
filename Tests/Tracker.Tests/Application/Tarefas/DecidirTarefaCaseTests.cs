using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
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

public class DecidirTarefaCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveAprovarAtribuirSolicitanteRegistrarHistoricoENotificar()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Solicitacao.Id, aprovar: true);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal((int)StatusSolicitacaoObtencaoTarefa.Aprovada, resultado.Dados!.Status);
        Assert.Equal(cenario.Solicitante.Id, cenario.Tarefa.UsuarioId);
        Assert.Equal(cenario.Solicitante.Codigo, cenario.Tarefa.UsuarioCodigo);
        Assert.Equal((int)DestinoInicialTarefa.Usuario, cenario.Tarefa.DestinoInicial);
        Assert.Null(cenario.Tarefa.DepartamentoId);
        Assert.Null(cenario.Tarefa.CargoId);
        Assert.Equal(5, cenario.Tarefa.TarefaEstadoId);
        Assert.False(cenario.Tarefa.ExigeAprovacaoParaObter);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Mensagem_SolicitacaoAprovada);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(TipoMovimentacaoTarefa.AprovacaoObtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(cenario.Aprovador.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheAprovacao,
                "Usuario Solicitante",
                "Iniciada"),
            movimentacaoRegistrada.Descricao);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.Is<PublicarNotificacaoInternaRequest>(request =>
                    request.DestinatarioCodigo == cenario.Solicitante.Codigo &&
                    request.TipoEvento == "SolicitacaoObtencaoAprovada"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRecusarPreservarTarefaRegistrarHistoricoENotificar()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var cenario = CriarCenario();
        var estadoAnterior = cenario.Tarefa.TarefaEstadoId;
        var exigeAprovacaoAnterior = cenario.Tarefa.ExigeAprovacaoParaObter;
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Solicitacao.Id, aprovar: false);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal((int)StatusSolicitacaoObtencaoTarefa.Recusada, resultado.Dados!.Status);
        Assert.Null(cenario.Tarefa.UsuarioId);
        Assert.Equal(estadoAnterior, cenario.Tarefa.TarefaEstadoId);
        Assert.Equal(exigeAprovacaoAnterior, cenario.Tarefa.ExigeAprovacaoParaObter);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Mensagem_SolicitacaoRecusada);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(TipoMovimentacaoTarefa.RecusaObtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheRecusa,
                "Usuario Solicitante"),
            movimentacaoRegistrada.Descricao);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.Is<PublicarNotificacaoInternaRequest>(request =>
                    request.DestinatarioCodigo == cenario.Solicitante.Codigo &&
                    request.TipoEvento == "SolicitacaoObtencaoRecusada"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveFalharSemEfeitosPosterioresQuandoDecisaoNaoForPersistida()
    {
        var cenario = CriarCenario();
        cenario.Solicitacoes
            .Setup(repository => repository.AprovarAsync(
                cenario.Solicitacao.Id,
                cenario.Aprovador.Id,
                cenario.Aprovador.Codigo))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Solicitacao.Id, aprovar: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_DecidirSolicitacao);
        cenario.Solicitacoes.Verify(
            repository => repository.ObterPorIdAsync(It.IsAny<int>()),
            Times.Never);
        VerificarAusenciaDeEfeitosPosteriores(cenario);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarFalhaDaMovimentacaoSemNotificarSolicitante()
    {
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Solicitacao.Id, aprovar: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DeveManterSucessoQuandoNotificacaoConsultivaFalhar()
    {
        var cenario = CriarCenario();
        cenario.Publisher
            .Setup(publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Falha("Falha simulada"));

        var resultado = await cenario.Case.ExecutarAsync(cenario.Solicitacao.Id, aprovar: false);

        Assert.True(resultado.TeveSucesso);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CenarioDecisao CriarCenario()
    {
        var solicitante = new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Solicitante"
        };
        var aprovador = new Usuario
        {
            Id = 20,
            Codigo = "GST",
            Nome = "Gestor",
            Sobrenome = "Aprovador"
        };
        var tarefa = new Tarefa
        {
            Id = 30,
            DestinoInicial = (int)DestinoInicialTarefa.DepartamentoCargo,
            DepartamentoId = 100,
            DepartamentoCodigo = "DPT",
            Departamento = new Departamento { Id = 100, Codigo = "DPT" },
            CargoId = 200,
            CargoCodigo = "CRG",
            Cargo = new Cargo { Id = 200, Codigo = "CRG" },
            TarefaEstadoId = 2,
            EstadoDaTarefa = new TarefaEstado { Id = 2, Descricao = "Pendente de aprovação" },
            ExigeAprovacaoParaObter = true
        };
        var solicitacao = new SolicitacaoObtencaoTarefa
        {
            Id = 40,
            TarefaId = tarefa.Id,
            Tarefa = tarefa,
            SolicitanteId = solicitante.Id,
            SolicitanteCodigo = solicitante.Codigo,
            Solicitante = solicitante,
            AprovadorId = aprovador.Id,
            AprovadorCodigo = aprovador.Codigo,
            Aprovador = aprovador,
            Status = (int)StatusSolicitacaoObtencaoTarefa.Pendente,
            DataSolicitacao = new DateTime(2026, 8, 19, 10, 0, 0)
        };

        var solicitacoes = new Mock<ISolicitacaoObtencaoTarefaRepository>();
        solicitacoes
            .Setup(repository => repository.AprovarAsync(
                solicitacao.Id,
                aprovador.Id,
                aprovador.Codigo))
            .Callback(() =>
            {
                solicitacao.Status = (int)StatusSolicitacaoObtencaoTarefa.Aprovada;
                solicitacao.DataDecisao = DateTime.UtcNow;
                tarefa.AprovarObtencao(solicitante.Id, solicitante.Codigo);
                tarefa.EstadoDaTarefa = new TarefaEstado { Id = 5, Descricao = "Iniciada" };
            })
            .ReturnsAsync(true);
        solicitacoes
            .Setup(repository => repository.RecusarAsync(
                solicitacao.Id,
                aprovador.Id,
                aprovador.Codigo))
            .Callback(() =>
            {
                solicitacao.Status = (int)StatusSolicitacaoObtencaoTarefa.Recusada;
                solicitacao.DataDecisao = DateTime.UtcNow;
            })
            .ReturnsAsync(true);
        solicitacoes
            .Setup(repository => repository.ObterPorIdAsync(solicitacao.Id))
            .ReturnsAsync(solicitacao);

        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(aprovador));

        var tarefaMapper = new Mock<IToDtoMapper<Tarefa, TarefaDTO>>();
        tarefaMapper
            .Setup(mapper => mapper.MapToDto(tarefa))
            .Returns(() => new TarefaDTO
            {
                Id = tarefa.Id,
                UsuarioCodigo = tarefa.UsuarioCodigo,
                EstadoDaTarefa = new TarefaEstadoDTO
                {
                    Id = tarefa.TarefaEstadoId,
                    Descricao = tarefa.EstadoDaTarefa.Descricao
                }
            });

        var movimentacoes = new Mock<ITarefaMovimentacaoRepository>();
        movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(true);

        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher
            .Setup(item => item.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Sucesso(
                new NotificacaoInternaResponse(
                    1000001,
                    "Tracker",
                    "SolicitacaoObtencaoAprovada",
                    "",
                    "",
                    null,
                    null,
                    false,
                    DateTimeOffset.UtcNow,
                    null)));

        var caseDeDecisao = new DecidirTarefaCase(
            new RegistrarDecisaoTarefaMovimentacaoCase(
                movimentacoes.Object,
                new TarefaMovimentacaoMapping()),
            new SolicitacaoObtencaoTarefaMapping(tarefaMapper.Object),
            solicitacoes.Object,
            new TarefaNotificacaoInternaCase(publisher.Object),
            usuarioService.Object);

        return new CenarioDecisao(
            caseDeDecisao,
            solicitacoes,
            movimentacoes,
            publisher,
            solicitante,
            aprovador,
            tarefa,
            solicitacao);
    }

    private static void VerificarAusenciaDeEfeitosPosteriores(CenarioDecisao cenario)
    {
        cenario.Movimentacoes.Verify(
            repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Never);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private sealed record CenarioDecisao(
        DecidirTarefaCase Case,
        Mock<ISolicitacaoObtencaoTarefaRepository> Solicitacoes,
        Mock<ITarefaMovimentacaoRepository> Movimentacoes,
        Mock<INotificacoesInternasPublisher> Publisher,
        Usuario Solicitante,
        Usuario Aprovador,
        Tarefa Tarefa,
        SolicitacaoObtencaoTarefa Solicitacao);
}
