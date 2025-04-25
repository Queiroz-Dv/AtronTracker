using Atron.Domain.Componentes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPropriedadeDeFluxoModuloRepository
    {
        Task<List<PropriedadeDeFluxoModulo>> ObterRelacionamentosTodosAsync();
        Task<bool> GravarAsync(PropriedadeDeFluxoModulo entidade);
    }
}