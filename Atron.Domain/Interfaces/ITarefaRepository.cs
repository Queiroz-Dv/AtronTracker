using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ITarefaRepository 
    {
        Task<Tarefa> ObterTarefaPorId(int id);

        Task<List<Tarefa>> ObterTodasTarefas();

        Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int id, string codigo);

        Task<bool> CriarTarefaAsync(Tarefa tarefa);

        Task<bool> AtualizarTarefaAsync(int id, Tarefa tarefa);

        Task<bool> ExcluirTarefaAsync(int id);

        Task<string> ObterDescricaoTarefaEstado(int tarefaEstadoId);
    }
}