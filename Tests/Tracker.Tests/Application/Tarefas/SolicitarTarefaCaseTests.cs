using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Policies.Tarefas;
using Application.Resolvers.Tarefas;
using Application.Resources;
using Application.UseCases.TarefaCases;
using Application.UseCases.TarefaCases.Movimentacao;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Contracts.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class SolicitarTarefaCaseTests
{
    [Fact]
    public async Task ExecutarAsync_DeveCriarSolicitacaoPendenteRegistrarMovimentacaoENotificarAprovador()
    {
        TarefaMovimentacao? movimentacaoRegistrada = null;
        var cenario = CriarCenario();
        var estadoAntesDaSolicitacao = cenario.Tarefa.TarefaEstadoId;
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(movimentacao => movimentacaoRegistrada = movimentacao)
            .ReturnsAsync(true);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Equal(
            (int)StatusSolicitacaoObtencaoTarefa.Pendente,
            resultado.Dados!.Status);
        Assert.Equal(estadoAntesDaSolicitacao, cenario.Tarefa.TarefaEstadoId);
        Assert.NotNull(cenario.Solicitacao.Valor);
        Assert.Equal(
            (int)StatusSolicitacaoObtencaoTarefa.Pendente,
            cenario.Solicitacao.Valor!.Status);
        Assert.Equal(cenario.Usuario.Id, cenario.Solicitacao.Valor.SolicitanteId);
        Assert.Equal(cenario.Aprovador.Id, cenario.Solicitacao.Valor.AprovadorId);
        Assert.NotNull(movimentacaoRegistrada);
        Assert.Equal(cenario.Tarefa.Id, movimentacaoRegistrada.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.SolicitacaoObtencao, movimentacaoRegistrada.Tipo);
        Assert.Equal(cenario.Usuario.Codigo, movimentacaoRegistrada.ResponsavelCodigo);
        Assert.Equal("Usuario Solicitante", movimentacaoRegistrada.ResponsavelNome);
        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheSolicitacao,
                "Gestor Aprovador"),
            movimentacaoRegistrada.Descricao);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.Is<PublicarNotificacaoInternaRequest>(request =>
                    request.DestinatarioCodigo == cenario.Aprovador.Codigo &&
                    request.TipoEvento == "SolicitacaoObtencaoRecebida"),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecutarAsync_DeveFalharQuandoNaoExistirAprovadorValido()
    {
        var cenario = CriarCenario();
        cenario.Usuarios
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(cenario.Aprovador.Codigo))
            .ReturnsAsync((Usuario)null!);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_AprovadorIndisponivel);
        cenario.Solicitacoes.Verify(
            repository => repository.CriarAsync(It.IsAny<SolicitacaoObtencaoTarefa>()),
            Times.Never);
        VerificarAusenciaDeEfeitosPosteriores(cenario);
    }

    [Fact]
    public async Task ExecutarAsync_DeveFalharQuandoJaExistirSolicitacaoPendente()
    {
        var cenario = CriarCenario();
        cenario.Solicitacoes
            .Setup(repository => repository.ExisteSolicitacaoPendenteParaTarefaAsync(cenario.Tarefa.Id))
            .ReturnsAsync(true);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_SolicitacaoPendenteExistente);
        cenario.Usuarios.Verify(
            repository => repository.ObterUsuarioPorCodigoAsync(It.IsAny<string>()),
            Times.Never);
        cenario.Solicitacoes.Verify(
            repository => repository.CriarAsync(It.IsAny<SolicitacaoObtencaoTarefa>()),
            Times.Never);
        VerificarAusenciaDeEfeitosPosteriores(cenario);
    }

    [Fact]
    public async Task ExecutarAsync_DeveFalharQuandoSolicitacaoNaoForPersistida()
    {
        var cenario = CriarCenario();
        cenario.Solicitacoes
            .Setup(repository => repository.CriarAsync(It.IsAny<SolicitacaoObtencaoTarefa>()))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_CriarSolicitacao);
        cenario.Solicitacoes.Verify(
            repository => repository.ObterPorIdAsync(It.IsAny<int>()),
            Times.Never);
        VerificarAusenciaDeEfeitosPosteriores(cenario);
    }

    [Fact]
    public async Task ExecutarAsync_DevePropagarFalhaDaMovimentacaoSemNotificarAprovador()
    {
        var cenario = CriarCenario();
        cenario.Movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(false);

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

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

        var resultado = await cenario.Case.ExecutarAsync(cenario.Tarefa.Id);

        Assert.True(resultado.TeveSucesso);
        cenario.Publisher.Verify(
            publisher => publisher.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static CenarioSolicitacao CriarCenario()
    {
        var usuario = new Usuario
        {
            Id = 10,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Solicitante",
            GestorImediatoCodigo = "GST"
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
            Identificador = 123456,
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" },
            ExigeAprovacaoParaObter = false
        };
        var solicitacao = new SolicitacaoCapturada();

        var tarefas = new Mock<ITarefaRepository>();
        tarefas.Setup(repository => repository.ObterTarefaPorId(tarefa.Id)).ReturnsAsync(tarefa);
        tarefas
            .Setup(repository => repository.PossuiResponsabilidadeGestaoAsync(usuario.Id, usuario.Codigo))
            .ReturnsAsync(false);

        var solicitacoes = new Mock<ISolicitacaoObtencaoTarefaRepository>();
        solicitacoes
            .Setup(repository => repository.ExisteSolicitacaoPendenteParaTarefaAsync(tarefa.Id))
            .ReturnsAsync(false);
        solicitacoes
            .Setup(repository => repository.CriarAsync(It.IsAny<SolicitacaoObtencaoTarefa>()))
            .Callback<SolicitacaoObtencaoTarefa>(valor =>
            {
                valor.Id = 40;
                solicitacao.Valor = valor;
            })
            .ReturnsAsync(true);
        solicitacoes
            .Setup(repository => repository.ObterPorIdAsync(40))
            .ReturnsAsync(() =>
            {
                solicitacao.Valor!.Tarefa = tarefa;
                solicitacao.Valor.Solicitante = usuario;
                solicitacao.Valor.Aprovador = aprovador;
                return solicitacao.Valor;
            });

        var usuarioService = new Mock<IUsuarioService>();
        usuarioService
            .Setup(service => service.ObterUsuarioAtual())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var usuarios = new Mock<IUsuarioRepository>();
        usuarios
            .Setup(repository => repository.ObterUsuarioPorCodigoAsync(aprovador.Codigo))
            .ReturnsAsync(aprovador);

        var mapper = new Mock<IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO>>();
        mapper
            .Setup(item => item.MapToDto(It.IsAny<SolicitacaoObtencaoTarefa>()))
            .Returns(new SolicitacaoObtencaoTarefaDTO
            {
                Id = 40,
                TarefaId = tarefa.Id,
                Status = (int)StatusSolicitacaoObtencaoTarefa.Pendente
            });

        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher
            .Setup(item => item.PublicarAsync(
                It.IsAny<PublicarNotificacaoInternaRequest>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Sucesso(
                new NotificacaoInternaResponse(
                    1000001,
                    "Tracker",
                    "SolicitacaoObtencaoRecebida",
                    "",
                    "",
                    null,
                    null,
                    false,
                    DateTimeOffset.UtcNow,
                    null)));

        var movimentacoes = new Mock<ITarefaMovimentacaoRepository>();
        movimentacoes
            .Setup(repository => repository.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .ReturnsAsync(true);

        var caseDeSolicitacao = new SolicitarTarefaCase(
            usuarioService.Object,
            tarefas.Object,
            new TarefaObtencaoPolicy(),
            solicitacoes.Object,
            new RegistrarSolicitacaoTarefaMovimentacaoCase(
                movimentacoes.Object,
                new TarefaMovimentacaoMapping()),
            mapper.Object,
            new TarefaNotificacaoInternaCase(publisher.Object),
            new AprovadorObtencaoTarefaResolver(usuarios.Object));

        return new CenarioSolicitacao(
            caseDeSolicitacao,
            tarefas,
            solicitacoes,
            usuarios,
            movimentacoes,
            publisher,
            usuario,
            aprovador,
            tarefa,
            solicitacao);
    }

    private static void VerificarAusenciaDeEfeitosPosteriores(CenarioSolicitacao cenario)
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

    private sealed class SolicitacaoCapturada
    {
        public SolicitacaoObtencaoTarefa? Valor { get; set; }
    }

    private sealed record CenarioSolicitacao(
        SolicitarTarefaCase Case,
        Mock<ITarefaRepository> Tarefas,
        Mock<ISolicitacaoObtencaoTarefaRepository> Solicitacoes,
        Mock<IUsuarioRepository> Usuarios,
        Mock<ITarefaMovimentacaoRepository> Movimentacoes,
        Mock<INotificacoesInternasPublisher> Publisher,
        Usuario Usuario,
        Usuario Aprovador,
        Tarefa Tarefa,
        SolicitacaoCapturada Solicitacao);
}
