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

        Task<bool> CriarCargoAsync(Cargo cargo);

        Task<bool> AtualizarCargoAsync(Cargo cargo);

        Task<bool> RemoverCargoAsync(Cargo cargo);        

        Task<IEnumerable<Cargo>> ObterCargosPorDepartamento(int departamentoId, string departamentoCodigo);
    }
}