using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface IAuditoriaService
    {
        Task<Resultado> AuditarAsync(Auditoria auditoria);

        Task<Resultado> AtualizarAsync(Auditoria auditoria);

        Task<Resultado<Auditoria>> ObterAuditoriaPorIdAsync(int id);
    }
}