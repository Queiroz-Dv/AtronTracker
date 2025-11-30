using Shared.Application.DTOS.Common;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IHistoricoService
    {        
        Task<Resultado<IList<Historico>>> ObterPorChaveServiceAsync(IHistoricoDTO historicoDTO);

        Task<Resultado> RegistrarServiceAsync(IHistoricoDTO historico);            
    }
}