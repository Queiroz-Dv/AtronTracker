using Application.DTO;
using Application.Interfaces.Services;
using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaObtencaoServiceTests
{
    [Fact]
    public async Task SolicitarAsync_DeveCriarSolicitacaoENotificarGestorParaUsuarioSemResponsabilidadeDeGestao()
    {
        var usuario = new Usuario { Id = 10, Codigo = "USR" };
        var gestor = new Usuario { Id = 20, Codigo = "GST" };
        var tarefa = new Tarefa
        {
            Id = 30,
            TarefaEstadoId = 1,
            ExigeAprovacaoParaObter = false
        };
        SolicitacaoObtencaoTarefa solicitacaoGravada = null!;

        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(item => item.ObterTarefaPorId(tarefa.Id)).ReturnsAsync(tarefa);
        tarefaRepository
            .Setup(item => item.PossuiResponsabilidadeGestaoAsync(usuario.Id, usuario.Codigo))
            .ReturnsAsync(false);

        var solicitacaoRepository = new Mock<ISolicitacaoObtencaoTarefaRepository>();
        solicitacaoRepository
            .Setup(item => item.ExisteSolicitacaoPendenteParaTarefaAsync(tarefa.Id))
            .ReturnsAsync(false);
        solicitacaoRepository
            .Setup(item => item.CriarAsync(It.IsAny<SolicitacaoObtencaoTarefa>()))
            .Callback<SolicitacaoObtencaoTarefa>(solicitacao =>
            {
                solicitacao.Id = 40;
                solicitacaoGravada = solicitacao;
            })
            .ReturnsAsync(true);
        solicitacaoRepository
            .Setup(item => item.ObterPorIdAsync(40))
            .ReturnsAsync(() => solicitacaoGravada);

        var usuarioAtualService = new Mock<ITarefaUsuarioAtualService>();
        usuarioAtualService
            .Setup(item => item.ObterAsync())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var aprovadorResolver = new Mock<IAprovadorObtencaoTarefaResolver>();
        aprovadorResolver
            .Setup(item => item.ResolverAsync(usuario, tarefa))
            .ReturnsAsync(gestor);

        var mapeador = new Mock<ISolicitacaoObtencaoTarefaMapeador>();
        mapeador
            .Setup(item => item.MapearAsync(It.IsAny<SolicitacaoObtencaoTarefa>()))
            .ReturnsAsync(new SolicitacaoObtencaoTarefaDTO());

        var notificacao = new Mock<ITarefaNotificacaoInternaService>();
        var movimentacao = new Mock<ITarefaMovimentacaoService>();
        movimentacao
            .Setup(item => item.RegistrarSolicitacaoAsync(It.IsAny<SolicitacaoObtencaoTarefa>(), usuario))
            .ReturnsAsync(Resultado.Sucesso());
        var service = new TarefaObtencaoService(
            tarefaRepository.Object,
            solicitacaoRepository.Object,
            usuarioAtualService.Object,
            new TarefaObtencaoValidador(),
            aprovadorResolver.Object,
            mapeador.Object,
            notificacao.Object,
            Mock.Of<IAsyncApplicationMapService<TarefaDTO, Tarefa>>(),
            movimentacao.Object);

        var resultado = await service.SolicitarAsync(tarefa.Id);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(usuario.Id, solicitacaoGravada.SolicitanteId);
        Assert.Equal(gestor.Id, solicitacaoGravada.AprovadorId);
        notificacao.Verify(
            item => item.NotificarSolicitacaoRecebidaAsync(solicitacaoGravada),
            Times.Once);
    }

    [Fact]
    public async Task AssumirAsync_DeveBloquearUsuarioSemResponsabilidadeDeGestao()
    {
        var usuario = new Usuario { Id = 10, Codigo = "USR" };
        var tarefa = new Tarefa
        {
            Id = 30,
            TarefaEstadoId = 1,
            ExigeAprovacaoParaObter = false
        };
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository.Setup(item => item.ObterTarefaPorId(tarefa.Id)).ReturnsAsync(tarefa);
        tarefaRepository
            .Setup(item => item.PossuiResponsabilidadeGestaoAsync(usuario.Id, usuario.Codigo))
            .ReturnsAsync(false);

        var usuarioAtualService = new Mock<ITarefaUsuarioAtualService>();
        usuarioAtualService
            .Setup(item => item.ObterAsync())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(usuario));

        var service = new TarefaObtencaoService(
            tarefaRepository.Object,
            Mock.Of<ISolicitacaoObtencaoTarefaRepository>(),
            usuarioAtualService.Object,
            new TarefaObtencaoValidador(),
            Mock.Of<IAprovadorObtencaoTarefaResolver>(),
            Mock.Of<ISolicitacaoObtencaoTarefaMapeador>(),
            Mock.Of<ITarefaNotificacaoInternaService>(),
            Mock.Of<IAsyncApplicationMapService<TarefaDTO, Tarefa>>(),
            Mock.Of<ITarefaMovimentacaoService>());

        var resultado = await service.AssumirAsync(tarefa.Id);

        Assert.True(resultado.TeveFalha);
        tarefaRepository.Verify(
            item => item.AssumirTarefaAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()),
            Times.Never);
    }
}
