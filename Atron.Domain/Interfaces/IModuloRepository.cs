using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IModuloRepository
    {
        Task<IEnumerable<Modulo>> ObterTodosRepository();
        Task<Modulo> ObterPorIdRepository(int id);
        Task<Modulo> ObterPorCodigoRepository(string codigo);
        Task<bool> CriarModuloRepository(Modulo modulo);
        Task<Modulo> Atualizar(Modulo modulo);
        Task<bool> RemoverModuloRepository(Modulo modulo);
    }
}