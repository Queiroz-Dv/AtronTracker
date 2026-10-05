using Application.UseCases.UsuarioCases;
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
using Shared.Extensions;
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
        tarefa.DestinoInicial = DestinoInicialTarefa.Equipe.GetDescription();

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
        tarefa.DestinoInicial = DestinoInicialTarefa.Equipe.GetDescription();

        var resultado = await cenario.Case.ExecutarAsync(tarefa);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_DepartamentoEquipeIndefinido);
        cenario.Preparacao.Verify(
            service => service.PrepararParaPersistenciaAsync(It.IsAny<TarefaDTO>()),
            Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(999)]
    public async Task CriarAsync_DevePublicarTextoFinalDaNotificacaoInterna(int idInformado)
    {
        global::Application.Events.TarefaNotificacaoEvent? capturada = null;
        var cenario = CriarCenario(
            capturarNotificacao: notificacao => capturada = notificacao);
        var tarefa = CriarTarefaDto();
        tarefa.Id = idInformado;

        var resultado = await cenario.Case.ExecutarAsync(tarefa);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(cenario.Tarefa.Id, tarefa.Id);
        Assert.NotNull(capturada);
        Assert.Equal(TarefaResource.Titulo_TarefaAtribuida, capturada.NotificacaoInterna.Titulo);
        Assert.Equal("A tarefa 42 foi atribuída a você.", capturada.NotificacaoInterna.Mensagem);
        Assert.Equal(AtronNotificacoes.Domain.Enums.ENotificacaoModulos.Tracker, capturada.NotificacaoInterna.ModuloOrigem);
        Assert.Equal("TarefaAtribuida", capturada.NotificacaoInterna.TipoEvento);
        Assert.Equal("/atron/tarefas/editar/42", capturada.NotificacaoInterna.UrlDestino);
        Assert.Equal("tarefa:42", capturada.NotificacaoInterna.ReferenciaExterna);
        Assert.Equal("tracker:tarefa:42:TarefaAtribuida:USR", capturada.NotificacaoInterna.ChaveIdempotencia);
        Assert.Equal("tracker:TarefaAtribuida:tarefa:42", capturada.NotificacaoInterna.CorrelacaoId);
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
        cenario.UsuarioService.Verify(service => service.ObterAsync(), Times.Once);

        cenario.Tarefas.Verify(
            repository => repository.CriarTarefaAsync(It.IsAny<Tarefa>()),
            Times.Never);
        cenario.Movimentacoes.Verify(
            repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()),
            Times.Never);
        
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<global::Application.Events.TarefaNotificacaoEvent>(),
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
        
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<global::Application.Events.TarefaNotificacaoEvent>(),
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
        
        cenario.Publisher.Verify(
            service => service.PublicarAsync(
                It.IsAny<global::Application.Events.TarefaNotificacaoEvent>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    private static CenarioCriacao CriarCenario(
        
        Action<global::Application.Events.TarefaNotificacaoEvent>? capturarNotificacao = null,
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

        var preparacao = new Mock<global::Application.Services.EntitiesServices.Tarefas.TarefaPreparacaoService>();
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

        var publisher = new Mock<Shared.Application.Messaging.IEventBus>();
        publisher
            .Setup(service => service.PublicarAsync(It.IsAny<global::Application.Events.TarefaNotificacaoEvent>(), It.IsAny<CancellationToken>()))
            .Callback<global::Application.Events.TarefaNotificacaoEvent, CancellationToken>((valor, _) => capturarNotificacao?.Invoke(valor))
            .Returns(System.Threading.Tasks.ValueTask.CompletedTask);
        var notificacao = publisher.Object;

        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        movimentacaoRepository
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(true);
        var movimentacao = new CriarTarefaMovimentacaoCase(
            movimentacaoRepository.Object,
            new TarefaMovimentacaoMapping());

        var usuarioAtual = new Mock<ObterUsuarioCase>();
        usuarioAtual
            .Setup(service => service.ObterAsync())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(responsavel));

        return new CenarioCriacao(
            new CriarTarefaCase(
                tarefaRepository.Object,
                preparacao.Object,
                
                notificacao,
                usuarioAtual.Object,
                movimentacao),
            preparacao,
            tarefaRepository,
            movimentacaoRepository,
            usuarioAtual,
            
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
        Mock<global::Application.Services.EntitiesServices.Tarefas.TarefaPreparacaoService> Preparacao,
        Mock<ITarefaRepository> Tarefas,
        Mock<ITarefaMovimentacaoRepository> Movimentacoes,
        Mock<ObterUsuarioCase> UsuarioService,
        
        Mock<Shared.Application.Messaging.IEventBus> Publisher,
        Tarefa Tarefa,
        Usuario Responsavel);
}





