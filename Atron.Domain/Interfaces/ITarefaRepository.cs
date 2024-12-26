using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ITarefaRepository : IRepository<Tarefa>
    {
        Task<Tarefa> ObterTarefaPorId(int id);

        Task<List<Tarefa>> ObterTodasTarefas();

        Task<Tarefa> CriarTarefaAsync(Tarefa tarefa);

        Task<Tarefa> AtualizarTarefaAsync(Tarefa tarefa);
    }
}