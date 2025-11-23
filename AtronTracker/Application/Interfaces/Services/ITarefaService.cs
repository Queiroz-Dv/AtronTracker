using Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITarefaService
    {
        Task<List<TarefaDTO>> ObterTodosAsync();

        Task CriarAsync(TarefaDTO tarefaDTO);

        Task AtualizarAsync(int id, TarefaDTO tarefaDTO);

        Task ExcluirAsync(string id);
        Task<TarefaDTO> ObterPorId(int id);
    }
}