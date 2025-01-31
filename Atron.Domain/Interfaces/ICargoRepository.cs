using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface ICargoRepository : IRepository<Cargo>
    {
        Task<IEnumerable<Cargo>> ObterCargosAsync();

        Task<Cargo> ObterCargoPorIdAsync(int? id);
        Task<Cargo> ObterCargoPorCodigoAsync(string codigo);

        Task<Cargo> ObterCargoPorCodigoAsyncAsNoTracking(string codigo);

        Task<Cargo> CriarCargoAsync(Cargo cargo);

        Task<Cargo> AtualizarCargoAsync(Cargo cargo);

        Task<Cargo> RemoverCargoAsync(Cargo cargo);

        bool CargoExiste(string codigo);
        Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo);
    }
}