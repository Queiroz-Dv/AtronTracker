using Domain.Componentes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPropriedadeDeFluxoRepository
    {
        Task<IEnumerable<PropriedadesDeFluxo>> ObterTodasPropriedades();
        Task<PropriedadesDeFluxo> ObterPropriedadePorId(int id);
        Task<PropriedadesDeFluxo> ObterPropriedadePorCodigo(string codigo);
    }
}