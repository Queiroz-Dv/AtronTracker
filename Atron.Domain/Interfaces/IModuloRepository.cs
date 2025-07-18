using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IModuloRepository : IRepository<Modulo>
    {
        Task<IEnumerable<Modulo>> ObterTodosRepository();
        Task<Modulo> ObterPorIdRepository(int id);
        Task<Modulo> ObterPorCodigoRepository(string codigo);                       
    }
}