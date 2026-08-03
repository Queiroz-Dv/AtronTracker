using Domain.Entities;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaNotificacaoInternaService
    {
        Task NotificarAtribuicaoAsync(Tarefa tarefa, Usuario usuario);

        Task NotificarObtencaoAsync(Tarefa tarefa, Usuario usuario);

        Task NotificarSolicitacaoRecebidaAsync(SolicitacaoObtencaoTarefa solicitacao);

        Task NotificarDecisaoSolicitacaoAsync(SolicitacaoObtencaoTarefa solicitacao, bool aprovada);
    }
}
