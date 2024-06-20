using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ICargoRepository
    {
        Task<IEnumerable<Cargo>> ObterCargosAsync();

        Task<Cargo> ObterCargoPorIdAsync(int? id);
        Task<Cargo> ObterCargoPorCodigoAsync(string codigo);

        Task<Cargo> CriarCargoAsync(Cargo cargo);

        Task<Cargo> AtualizarCargoAsync(Cargo cargo);

        Task<Cargo> RemoverCargoAsync(Cargo cargo);
    }
}