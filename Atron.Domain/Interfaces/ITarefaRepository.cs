using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ITarefaRepository
    {
        Task<IEnumerable<Tarefa>> ObterTarefasAsync();

        Task<Tarefa> ObterTarefaPorIdAsync(int? id);

        Task<Tarefa> CriarTarefaAsync(Tarefa tarefa);

        Task<Tarefa> AtualizarTarefaAsync(Tarefa tarefa);

        Task<Tarefa> RemoverTarefaAsync(Tarefa tarefa);
    }
}