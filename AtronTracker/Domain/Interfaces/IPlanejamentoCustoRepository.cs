using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPlanejamentoCustoRepository
    {
        Task<IEnumerable<PlanejamentoCusto>> ObterTodosAsync();

        Task<IEnumerable<PlanejamentoCusto>> ObterPorAnoAsync(int ano);

        Task<PlanejamentoCusto> ObterPorCodigoAsync(string codigo);

        Task<PlanejamentoCusto> ObterPorDepartamentoEAnoAsync(int departamentoId, string departamentoCodigo, int ano);

        Task<PlanejamentoCusto> ObterPorCodigoAsNoTrackingAsync(string codigo);

        Task<bool> ExisteCodigoAsync(string codigo);

        Task<bool> ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(int departamentoId, string departamentoCodigo, int anoMinimo);

        Task<bool> ExisteCargoEmPlanejamentoAtualOuFuturoAsync(int cargoId, string cargoCodigo, int departamentoId, string departamentoCodigo, int anoMinimo);

        Task<bool> CriarAsync(PlanejamentoCusto planejamentoCusto);

        Task<bool> AtualizarAsync(PlanejamentoCusto planejamentoCusto);

        Task<bool> RemoverAsync(PlanejamentoCusto planejamentoCusto);
    }
}
