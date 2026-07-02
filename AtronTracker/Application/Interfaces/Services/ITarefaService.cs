using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITarefaService
    {
        Task<Resultado<List<TarefaDTO>>> ObterTodosAsync();

        Task<Resultado<List<TarefaEstadoDTO>>> ObterEstadosAsync();

        Task<Resultado<TarefaDTO>> CriarAsync(TarefaDTO tarefaDTO);

        Task<Resultado<TarefaDTO>> AtualizarAsync(int id, TarefaDTO tarefaDTO);

        Task<Resultado> ExcluirAsync(string id);

        Task<Resultado<TarefaDTO>> ObterPorId(int id);
    }
}
