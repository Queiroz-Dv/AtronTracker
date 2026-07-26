using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaObtencaoValidador : ITarefaObtencaoValidador
    {
        private const int EstadoFinalizadaId = 4;

        public Resultado ValidarAssuncao(Tarefa tarefa, bool possuiResponsabilidadeGestao)
        {
            var acesso = ValidarTarefaDisponivel(tarefa, TarefaResource.Erro_TarefaFinalizadaNaoPodeSerAssumida);
            if (acesso.TeveFalha)
                return acesso;

            return !possuiResponsabilidadeGestao || tarefa.ExigeAprovacaoParaObter
                ? Resultado.Falha(TarefaResource.Erro_TarefaExigeSolicitacaoObtencao)
                : Resultado.Sucesso();
        }

        public Resultado ValidarSolicitacao(Tarefa tarefa, bool possuiResponsabilidadeGestao)
        {
            var acesso = ValidarTarefaDisponivel(tarefa, TarefaResource.Erro_TarefaFinalizadaNaoPodeSerSolicitada);
            if (acesso.TeveFalha)
                return acesso;

            return possuiResponsabilidadeGestao && !tarefa.ExigeAprovacaoParaObter
                ? Resultado.Falha(TarefaResource.Erro_TarefaNaoExigeAprovacao)
                : Resultado.Sucesso();
        }

        private static Resultado ValidarTarefaDisponivel(Tarefa tarefa, string mensagemTarefaFinalizada)
        {
            if (tarefa.UsuarioId.HasValue)
                return Resultado.Falha(TarefaResource.Erro_TarefaJaPossuiUsuarioResponsavel);

            if (tarefa.TarefaEstadoId == EstadoFinalizadaId)
                return Resultado.Falha(mensagemTarefaFinalizada);

            return Resultado.Sucesso();
        }
    }
}
