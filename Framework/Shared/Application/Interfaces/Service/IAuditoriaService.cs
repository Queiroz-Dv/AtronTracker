using Shared.Application.DTOS.Common;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IAuditoriaService
    {      
        Task<Resultado> RegistrarAuditoriaAsync(AuditoriaDTO auditoria);
               
        Task<Resultado> RegistrarAlteracaoAuditoriaAsync(AuditoriaDTO auditoriaDTO);
        
        Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuarioLogado = null);

        Task<Resultado> RegistrarRemocaoAsync(string codigoRegistro, string usuarioLogado = null, string historicoDescricao = null);

        Task<Resultado<Auditoria>> ObterPorCodigoRegistro(string codigoRegistro);
    }
}