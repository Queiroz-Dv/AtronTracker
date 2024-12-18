using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ITarefaRepository : IRepository<Tarefa> {

        Task<List<Tarefa>> ObterTodasTarefasComEstado();    
    }
}