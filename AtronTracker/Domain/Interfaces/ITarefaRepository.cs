using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITarefaRepository : IRepository<Tarefa>
    {
        Task<Tarefa> ObterTarefaPorId(int id);

        Task<List<Tarefa>> ObterTodasTarefas();

        Task<IEnumerable<Tarefa>> ObterTodasTarefasPorUsuario(int id, string codigo);

        Task<IEnumerable<Tarefa>> ObterTarefasAtivasPorUsuarioAsync(int usuarioId, string usuarioCodigo);

        Task<IEnumerable<Tarefa>> ObterTarefasAtivasPorSubordinadosDiretosAsync(int gestorId, string gestorCodigo);       

        Task<IEnumerable<Tarefa>> ObterTarefasAtivasDisponiveisParaUsuarioAsync(Usuario usuario);

        Task<bool> AssumirTarefaAsync(int tarefaId, int usuarioId, string usuarioCodigo);

        Task<bool> CriarTarefaAsync(Tarefa tarefa);

        Task<bool> AtualizarTarefaAsync(int id, Tarefa tarefa);        
    }
}
