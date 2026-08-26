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
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class CriarTarefaTests
{
    [Fact]
    public async Task CriarAsync_DeveUsarDepartamentoDoResponsavelQuandoDestinoForEquipe()
    {
        TarefaDTO? tarefaPreparada = null;
        var cenario = CriarCenario(capturarTarefa: tarefa => tarefaPreparada = tarefa);
        cenario.Responsavel.UsuarioCargoDepartamentos =
        [
            new UsuarioCargoDepartamento
            {
                DepartamentoId = 10,
                DepartamentoCodigo = "ADM"
            }
        ];
        var tarefa = CriarTarefaDto();
        tarefa.DestinoInicial = (int)DestinoInicialTarefa.Equipe;

        var resultado = await cenario.Case.ExecutarAsync(tarefa);

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.NotNull(tarefaPreparada);
        Assert.Equal("ADM", tarefaPreparada.DepartamentoCodigo);
        Assert.Null(tarefaPreparada.UsuarioCodigo);
        Assert.Null(tarefaPreparada.CargoCodigo);
    }

    [Fact]
    public async Task CriarAsync_DeveFalharQuandoDestinoForEquipeEUsuarioPossuirMaisDeUmDepartamento()
    {
        var cenario = CriarCenario();
        cenario.Responsavel.UsuarioCargoDepartamentos =
        [
            new UsuarioCargoDepartamento { DepartamentoId = 10, DepartamentoCodigo = "ADM" },
            new UsuarioCargoDepartamento { DepartamentoId = 20, DepartamentoCodigo = "FIN" }
        ];
        var tarefa = CriarTarefaDto();
        tarefa.DestinoInicial = (int)DestinoInicialTarefa.Equipe;

        var resultado = await cenario.Case.ExecutarAsync(tarefa);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_DepartamentoEquipeIndefinido);
        cenario.Preparacao.Verify(
            service => service.PrepararParaPersistenciaAsync(It.IsAny<TarefaDTO>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DevePublicarTextoFinalDaNotificacaoInterna()
    {
        PublicarNotificacaoInternaRequest? capturada = null;
        var cenario = CriarCenario(
            capturarNotificacao: notificacao => capturada = notificacao);

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(capturada);
        Assert.Equal(TarefaResource.Titulo_TarefaAtribuida, capturada.Titulo);
        Assert.Equal("A tarefa 42 foi atribuída a você.", capturada.Mensagem);
        Assert.Equal("Tracker", capturada.ModuloOrigem);
        Assert.Equal("TarefaAtribuida", capturada.TipoEvento);
        Assert.Equal("/atron/tarefas/editar/42", capturada.UrlDestino);
    }

    [Fact]
    public async Task CriarAsync_DeveManterSucessoEAdicionarAvisoQuandoEmailFalha()
    {
        var cenario = CriarCenario(
            resultadoEmail: Resultado.Falha("Falha simulada"));

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Aviso_EmailNotificacaoNaoEnviado);
    }

    [Fact]
    public async Task CriarAsync_DeveRegistrarMovimentacaoDeCriacaoComContextoCompleto()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);
        var inicio = DateTime.UtcNow;

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(cenario.Tarefa.Id, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Criacao, movimentacaoRegistrada.Tipo);
        Assert.Equal(cenario.Responsavel.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal("Responsavel Teste", movimentacaoRegistrada.ResponsavelNome);
        Assert.Equal(
            string.Format(TarefaResource.Historico_DetalheCriacao, "Aberta"),
            movimentacaoRegistrada.Descricao);
        Assert.InRange(movimentacaoRegistrada.DataOcorrencia, inicio, DateTime.UtcNow);
    }

    [Fact]
    public async Task CriarAsync_DeveEncerrarFluxoQuandoPreparacaoFalhar()
    {
        var cenario = CriarCenario();
        cenario.Preparacao
            .Setup(service => service.PrepararParaPersistenciaAsync(It.IsAny<TarefaDTO>()))
            .ReturnsAsync(Resultado<Tarefa>.Falha("Falha de preparação"));

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == "Falha de preparação");
        cenario.UsuarioService.Verify(service => service.ObterUsuarioAtual(), Times.Never);

        cenario.Tarefas.Verify(
            repository => repository.CriarTarefaAsync(It.IsAny<Tarefa>()),
            Times.Never);
        cenario.Movimentacoes.Verify(
            repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Never);
        cenario.Email.Verify(
            service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), It.IsAny<UsuarioDTO>()),
            Times.Never);
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveEncerrarFluxoQuandoPersistenciaDaTarefaFalhar()
    {
        var cenario = CriarCenario();
        cenario.Tarefas
            .Setup(repository => repository.CriarTarefaAsync(cenario.Tarefa))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_GravarTarefa);
        cenario.Movimentacoes.Verify(
            repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Never);
        cenario.Email.Verify(
            service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), It.IsAny<UsuarioDTO>()),
            Times.Never);
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_DeveEncerrarFluxoQuandoPersistenciaDaMovimentacaoFalhar()
    {
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_RegistrarMovimentacao);
        cenario.Tarefas.Verify(
            repository => repository.CriarTarefaAsync(cenario.Tarefa),
            Times.Once);
        cenario.Email.Verify(
            service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), It.IsAny<UsuarioDTO>()),
            Times.Never);
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CenarioCriacao CriarCenario(
        Resultado? resultadoEmail = null,
        Action<PublicarNotificacaoInternaRequest>? capturarNotificacao = null,
        Action<TarefaDTO>? capturarTarefa = null)
    {
        var usuario = new UsuarioDTO { Id = 7, Codigo = "USR", Nome = "Usuario" };
        var responsavel = new Usuario
        {
            Id = 8,
            Codigo = "RESP",
            Nome = "Responsavel",
            Sobrenome = "Teste"
        };
        var entidade = new Tarefa
        {
            Id = 42,
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Aberta" }
        };

        var preparacao = new Mock<ITarefaPreparacaoService>();
        preparacao
            .Setup(service => service.PrepararParaPersistenciaAsync(It.IsAny<TarefaDTO>()))
            .ReturnsAsync((TarefaDTO tarefa) =>
            {
                tarefa.Usuario = usuario;
                capturarTarefa?.Invoke(tarefa);
                return Resultado<Tarefa>.Sucesso(entidade);
            });

        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(repository => repository.CriarTarefaAsync(entidade)).ReturnsAsync(true);

        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher
            .Setup(service => service.PublicarAsync(It.IsAny<PublicarNotificacaoInternaRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PublicarNotificacaoInternaRequest, CancellationToken>((valor, _) => capturarNotificacao?.Invoke(valor))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Sucesso(new NotificacaoInternaResponse(
                1000001, "Tracker", "TarefaAtribuida", "", "", null, null, false, DateTimeOffset.UtcNow, null)));
        var notificacao = new TarefaNotificacaoInternaCase(publisher.Object);

        var email = new Mock<ITarefaNotificacaoService>();
        email
            .Setup(service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), usuario))
            .ReturnsAsync(resultadoEmail ?? Resultado.Sucesso());

        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        movimentacaoRepository
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(true);
        var movimentacao = new CriarTarefaMovimentacaoCase(
            movimentacaoRepository.Object,
            new TarefaMovimentacaoMapping());

        var usuarioAtual = new Mock<IUsuarioService>();
        usuarioAtual
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(responsavel));

        return new CenarioCriacao(
            new CriarTarefaCase(
                tarefaRepository.Object,
                preparacao.Object,
                email.Object,
                notificacao,
                usuarioAtual.Object,
                movimentacao),
            preparacao,
            tarefaRepository,
            movimentacaoRepository,
            usuarioAtual,
            email,
            publisher,
            entidade,
            responsavel);
    }

    private static TarefaDTO CriarTarefaDto()
    {
        return new TarefaDTO
        {
            Titulo = "Tarefa teste",
            Conteudo = "Conteudo",
            DataInicial = new DateTime(2026, 7, 10),
            DataFinal = new DateTime(2026, 7, 12),
            EstadoDaTarefa = new TarefaEstadoDTO { Id = 1, Descricao = "Aberta" }
        };
    }

    private sealed record CenarioCriacao(
        CriarTarefaCase Case,
        Mock<ITarefaPreparacaoService> Preparacao,
        Mock<ITarefaRepository> Tarefas,
        Mock<ITarefaMovimentacaoRepository> Movimentacoes,
        Mock<IUsuarioService> UsuarioService,
        Mock<ITarefaNotificacaoService> Email,
        Mock<INotificacoesInternasPublisher> Publisher,
        Tarefa Tarefa,
        Usuario Responsavel);
}
