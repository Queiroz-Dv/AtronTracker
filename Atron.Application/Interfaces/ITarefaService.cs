using Atron.Application.DTO;
using Notification.Interfaces.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ITarefaService : INotificationMessage
    {
        Task<List<TarefaDTO>> ObterTodosAsync();

        Task CriarAsync(TarefaDTO tarefaDTO);

        Task AtualizarAsync(TarefaDTO tarefaDTO);

        Task ExcluirAsync(int id);
    }
}