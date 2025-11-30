using Shared.Application.DTOS.Common;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IAuditoriaService
    {      
        Task<Resultado> RegistrarServiceAsync(IAuditoriaDTO auditoria);
               
        Task<Resultado> AtualizarServiceAsync(IAuditoriaDTO auditoriaDTO);
              
        Task<Resultado> RemoverServiceAsync(IAuditoriaDTO auditoriaDTO);

        Task<Resultado<Auditoria>> ObterPorChaveServiceAsync(IAuditoriaDTO documento);
    }
}