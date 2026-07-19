using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services.EntitiesServices;
using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Application.Resources.Tests;

public class TarefaServiceNotificacaoTests
{
    [Fact]
    public async Task CriarAsync_DevePersistirTextoFinalDaNotificacaoInterna()
    {
        NotificacaoInterna? capturada = null;
        var service = CriarService(
            Resultado.Sucesso(),
            notificacao => capturada = notificacao);

        var resultado = await service.CriarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(capturada);
        Assert.Equal(TarefaResource.Titulo_TarefaAtribuida, capturada.Titulo);
        Assert.Equal("A tarefa 123456 foi atribuída a você.", capturada.Mensagem);
        Assert.Equal("Tarefas", capturada.Modulo);
        Assert.Equal("TarefaAtribuida", capturada.TipoEvento);
        Assert.Equal("/atron/tarefas/editar/42", capturada.UrlDestino);
    }

    [Fact]
    public async Task CriarAsync_DeveManterSucessoEAdicionarAvisoQuandoEmailFalha()
    {
        var service = CriarService(Resultado.Falha("Falha simulada"));

        var resultado = await service.CriarAsync(CriarTarefaDto());

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Aviso_EmailNotificacaoNaoEnviado);
    }

    private static TarefaService CriarService(
        Resultado resultadoEmail,
        Action<NotificacaoInterna>? capturarNotificacao = null)
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

        var notificacaoPersistencia = new Mock<INotificacaoInternaService>();
        notificacaoPersistencia
            .Setup(service => service.CriarAsync(It.IsAny<NotificacaoInterna>()))
            .Callback<NotificacaoInterna>(valor => capturarNotificacao?.Invoke(valor))
            .ReturnsAsync(Resultado<NotificacaoInternaDTO>.Sucesso(new NotificacaoInternaDTO()));
        var notificacao = new TarefaNotificacaoInternaService(notificacaoPersistencia.Object);

        var email = new Mock<ITarefaNotificacaoService>();
        email
            .Setup(service => service.NotificarAtribuicaoAsync(It.IsAny<TarefaDTO>(), usuario))
            .ReturnsAsync(resultadoEmail);

        return new TarefaService(
            Mock.Of<IAsyncApplicationMapService<TarefaDTO, Tarefa>>(),
            tarefaRepository.Object,
            preparacao.Object,
            email.Object,
            notificacao,
            Mock.Of<ITarefaObtencaoService>(),
            Mock.Of<ITarefaUsuarioAtualService>(),
            Mock.Of<ITarefaConfiguracoesService>());
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
