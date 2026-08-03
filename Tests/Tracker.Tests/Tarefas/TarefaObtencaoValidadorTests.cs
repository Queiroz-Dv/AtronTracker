using Application.Resources;
using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaObtencaoValidadorTests
{
    private readonly TarefaObtencaoValidador _validador = new();

    [Fact]
    public void ValidarAssuncao_DeveAceitarGestorQuandoTarefaNaoExigeAprovacao()
    {
        var tarefa = CriarTarefa();

        var resultado = _validador.ValidarAssuncao(tarefa, possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveSucesso);
    }

    [Fact]
    public void ValidarAssuncao_DeveExigirSolicitacaoQuandoAprovacaoForObrigatoria()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = true;

        var resultado = _validador.ValidarAssuncao(tarefa, possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaExigeSolicitacaoObtencao);
    }

    [Fact]
    public void ValidarAssuncao_DeveExigirSolicitacaoParaUsuarioSemResponsabilidadeDeGestao()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = false;

        var resultado = _validador.ValidarAssuncao(tarefa, possuiResponsabilidadeGestao: false);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaExigeSolicitacaoObtencao);
    }

    [Fact]
    public void ValidarSolicitacao_DevePermitirUsuarioSemResponsabilidadeDeGestaoQuandoTarefaNaoExigeAprovacao()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = false;

        var resultado = _validador.ValidarSolicitacao(tarefa, possuiResponsabilidadeGestao: false);

        Assert.True(resultado.TeveSucesso);
    }

    [Fact]
    public void ValidarSolicitacao_DeveRecusarTarefaJaAtribuida()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = true;
        tarefa.UsuarioId = 99;

        var resultado = _validador.ValidarSolicitacao(tarefa, possuiResponsabilidadeGestao: false);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaJaPossuiUsuarioResponsavel);
    }

    private static Tarefa CriarTarefa()
    {
        return new Tarefa
        {
            DepartamentoId = 10,
            CargoId = 20,
            TarefaEstadoId = 1
        };
    }
}
