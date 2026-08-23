using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ITarefaService
    {
        Task<Resultado> CriarAsync(TarefaDTO tarefaDTO);

        Task<Resultado> AtualizarAsync(int id, TarefaDTO tarefaDTO);

        Task<Resultado> ExcluirAsync(string id);

        Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterTodosAsync();

        Task<Resultado<TarefaDTO>> ObterPorId(int id);
    }
}
