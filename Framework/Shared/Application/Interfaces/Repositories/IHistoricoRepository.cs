using Shared.Domain.Entities;

namespace Shared.Application.Interfaces.Repositories
{
    public interface IHistoricoRepository
    {
        Task<bool> AdicionarAsync(Historico historico);              

        Task<IEnumerable<Historico>> ListarPorContextoCodigoAsync(string contexto, string codigo);
    }
}