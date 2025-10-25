using Domain.Componentes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPropriedadeDeFluxoModuloRepository
    {
        Task<List<PropriedadeDeFluxoModulo>> ObterRelacionamentosTodosAsync();
        Task<bool> GravarAsync(PropriedadeDeFluxoModulo entidade);
    }
}