using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface ITarefaService
    {
        Task<List<TarefaDTO>> ObterTodosAsync();

        Task CriarAsync(TarefaDTO tarefaDTO);

        Task AtualizarAsync(TarefaDTO tarefaDTO);

        Task ExcluirAsync(string id);
        Task<TarefaDTO> ObterPorId(int id);
    }
}