using Application.Interfaces.Services;
using Application.Resources;
using Application.Extensions;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaObtencaoValidador : ITarefaObtencaoValidador
    {
        private const int EstadoFinalizadaId = 4;

        public Resultado ValidarAssuncao(Usuario usuario, Tarefa tarefa)
        {
            var acesso = ValidarTarefaDisponivel(usuario, tarefa, TarefaResource.Erro_TarefaFinalizadaNaoPodeSerAssumida);
            if (acesso.TeveFalha)
                return acesso;

            return tarefa.ExigeAprovacaoParaObter
                ? Resultado.Falha(TarefaResource.Erro_TarefaExigeSolicitacaoObtencao)
                : Resultado.Sucesso();
        }

        public Resultado ValidarSolicitacao(Usuario usuario, Tarefa tarefa)
        {
            var acesso = ValidarTarefaDisponivel(usuario, tarefa, TarefaResource.Erro_TarefaFinalizadaNaoPodeSerSolicitada);
            if (acesso.TeveFalha)
                return acesso;

            return !tarefa.ExigeAprovacaoParaObter
                ? Resultado.Falha(TarefaResource.Erro_TarefaNaoExigeAprovacao)
                : Resultado.Sucesso();
        }

        private static Resultado ValidarTarefaDisponivel(Usuario usuario, Tarefa tarefa, string mensagemTarefaFinalizada)
        {
            if (tarefa.UsuarioId.HasValue)
                return Resultado.Falha(TarefaResource.Erro_TarefaJaPossuiUsuarioResponsavel);

            if (tarefa.TarefaEstadoId == EstadoFinalizadaId)
                return Resultado.Falha(mensagemTarefaFinalizada);

            return UsuarioPodeObter(usuario, tarefa)
                ? Resultado.Sucesso()
                : Resultado.Falha(TarefaResource.Erro_UsuarioSemAcessoParaAssumir);
        }

        private static bool UsuarioPodeObter(Usuario usuario, Tarefa tarefa)
        {
            if (!tarefa.DepartamentoId.HasValue)
                return false;

            if (tarefa.Departamento?.GestorDepartamentoId == usuario.Id &&
                tarefa.Departamento?.GestorDepartamentoCodigo == usuario.Codigo)
                return true;

            var departamentoIds = usuario.ObterDepartamentoIdsParaTarefas();
            var cargoIds = usuario.ObterCargoIdsParaTarefas();

            return departamentoIds.Contains(tarefa.DepartamentoId.Value) &&
                   (!tarefa.CargoId.HasValue || cargoIds.Contains(tarefa.CargoId.Value));
        }
    }
}
