using Application.Policies.Tarefas;
using Application.Resources;
using Domain.Entities;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaObtencaoPolicyTests
{
    private readonly TarefaObtencaoPolicy _policy = new();

    [Fact]
    public void AvaliarAssuncao_DeveAceitarGestorQuandoTarefaNaoExigeAprovacao()
    {
        var tarefa = CriarTarefa();

        var resultado = _policy.AvaliarAssuncao(tarefa, possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveSucesso);
    }

    [Fact]
    public void AvaliarAssuncao_DeveExigirSolicitacaoQuandoAprovacaoForObrigatoria()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = true;

        var resultado = _policy.AvaliarAssuncao(tarefa, possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaExigeSolicitacaoObtencao);
    }

    [Fact]
    public void AvaliarAssuncao_DeveExigirSolicitacaoParaUsuarioSemResponsabilidadeDeGestao()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = false;

        var resultado = _policy.AvaliarAssuncao(tarefa, possuiResponsabilidadeGestao: false);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaExigeSolicitacaoObtencao);
    }

    [Fact]
    public void AvaliarAssuncao_DeveRecusarTarefaJaAtribuida()
    {
        var tarefa = CriarTarefa();
        tarefa.UsuarioId = 99;

        var resultado = _policy.AvaliarAssuncao(
            tarefa,
            possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaJaPossuiUsuarioResponsavel);
    }

    [Fact]
    public void AvaliarAssuncao_DeveRecusarTarefaFinalizada()
    {
        var tarefa = CriarTarefa();
        tarefa.TarefaEstadoId = 4;

        var resultado = _policy.AvaliarAssuncao(
            tarefa,
            possuiResponsabilidadeGestao: true);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == TarefaResource.Erro_TarefaFinalizadaNaoPodeSerAssumida);
    }

    [Fact]
    public void AvaliarSolicitacao_DevePermitirUsuarioSemResponsabilidadeDeGestaoQuandoTarefaNaoExigeAprovacao()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = false;

        var resultado = _policy.AvaliarSolicitacao(tarefa, possuiResponsabilidadeGestao: false);

        Assert.True(resultado.TeveSucesso);
    }

    [Fact]
    public void AvaliarSolicitacao_DeveRecusarTarefaJaAtribuida()
    {
        var tarefa = CriarTarefa();
        tarefa.ExigeAprovacaoParaObter = true;
        tarefa.UsuarioId = 99;

        var resultado = _policy.AvaliarSolicitacao(tarefa, possuiResponsabilidadeGestao: false);

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
