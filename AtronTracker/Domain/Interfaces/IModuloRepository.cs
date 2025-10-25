using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IModuloRepository
    {
        Task<IEnumerable<Modulo>> ObterTodosRepository();
        Task<Modulo> ObterPorIdRepository(int id);
        Task<Modulo> ObterPorCodigoRepository(string codigo);                       
    }
}