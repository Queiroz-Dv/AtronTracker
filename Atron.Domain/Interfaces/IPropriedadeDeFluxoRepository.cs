using Atron.Domain.Componentes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPropriedadeDeFluxoRepository : IRepository<PropriedadesDeFluxo>
    {
        Task<IEnumerable<PropriedadesDeFluxo>> ObterTodasPropriedades();
        Task<PropriedadesDeFluxo> ObterPropriedadePorId(int id);
        Task<PropriedadesDeFluxo> ObterPropriedadePorCodigo(string codigo);
    }
}