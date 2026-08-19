using AtronNotificacoes.Contracts.DTO;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaNotificacaoInternaService
    {
        Task PublicarAsync(PublicarNotificacaoInternaDto? notificacao);
    }
}
