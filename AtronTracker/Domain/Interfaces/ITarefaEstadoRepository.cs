using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITarefaEstadoRepository
    {
        Task<List<TarefaEstado>> ObterTodosAsync();

        Task<TarefaEstado> ObterPorIdAsync(int id);
    }
}
