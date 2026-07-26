using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.TarefaCases;
using AtronNotificacoes.Contracts;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class CriarTarefaTests
{
    [Fact]
    public async Task CriarAsync_DevePublicarTextoFinalDaNotificacaoInterna()
    {
        PublicarNotificacaoInternaRequest? capturada = null;
        var criarTarefa = CriarUseCase(
            Resultado.Sucesso(),
            notificacao => capturada = notificacao);

        var resultado = await criarTarefa.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(capturada);
        Assert.Equal(TarefaResource.Titulo_TarefaAtribuida, capturada.Titulo);
        Assert.Equal("A tarefa 123456 foi atribuída a você.", capturada.Mensagem);
        Assert.Equal("Tracker", capturada.ModuloOrigem);
        Assert.Equal("TarefaAtribuida", capturada.TipoEvento);
        Assert.Equal("/atron/tarefas/editar/42", capturada.UrlDestino);
    }

    [Fact]
    public async Task CriarAsync_DeveManterSucessoEAdicionarAvisoQuandoEmailFalha()
    {
        var criarTarefa = CriarUseCase(Resultado.Falha("Falha simulada"));

        var resultado = await criarTarefa.ExecutarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Aviso_EmailNotificacaoNaoEnviado);
    }

    private static CriarTarefa CriarUseCase(
        Resultado resultadoEmail,
        Action<PublicarNotificacaoInternaRequest>? capturarNotificacao = null)
    {
        var dto = CriarTarefaDto();
        var usuario = new Usuario { Id = 7, Codigo = "USR", Nome = "Usuario" };
        var entidade = new Tarefa { Id = 42, Identificador = 123456 };

        var preparacao = new Mock<ITarefaPreparacaoService>();
        preparacao
            .Setup(service => service.PrepararParaPersistenciaAsync(It.IsAny<TarefaDTO>()))
            .ReturnsAsync(Resultado<TarefaPreparada>.Sucesso(new TarefaPreparada(dto, entidade, usuario)));

        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(repository => repository.CriarTarefaAsync(entidade)).ReturnsAsync(true);

        var publisher = new Mock<INotificacoesInternasPublisher>();
        publisher
            .Setup(service => service.PublicarAsync(It.IsAny<PublicarNotificacaoInternaRequest>(), It.IsAny<CancellationToken>()))
            .Callback<PublicarNotificacaoInternaRequest, CancellationToken>((valor, _) => capturarNotificacao?.Invoke(valor))
            .ReturnsAsync(ResultadoPublicacaoNotificacaoInterna.Sucesso(new NotificacaoInternaResponse(
                1000001, "Tracker", "TarefaAtribuida", "", "", null, null, false, DateTimeOffset.UtcNow, null)));
        var notificacao = new TarefaNotificacaoInternaService(publisher.Object);

        var email = new Mock<ITarefaNotificacaoService>();
        email
            .Setup(service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), usuario))
            .ReturnsAsync(resultadoEmail);

        var movimentacao = new Mock<ITarefaMovimentacaoService>();
        movimentacao
            .Setup(service => service.RegistrarCriacaoAsync(entidade, usuario))
            .ReturnsAsync(Resultado.Sucesso());

        var usuarioAtual = new Mock<ITarefaUsuarioAtualService>();
        usuarioAtual
            .Setup(service => service.ObterAsync())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        return new CriarTarefa(
            tarefaRepository.Object,
            preparacao.Object,
            email.Object,
            notificacao,
            movimentacao.Object,
            usuarioAtual.Object);
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
}
