using Shared.Domain.Entities;

namespace Shared.Application.Interfaces.Repositories
{
    public interface IAuditoriaRepository
    {
        Task<bool> AdicionarAsync(Auditoria auditoria);

        Task<bool> AtualizarAsync(Auditoria auditoria); 

        Task<Auditoria?> ObterPorContextoCodigoAsync(string contexto, string codigo);
    }
}