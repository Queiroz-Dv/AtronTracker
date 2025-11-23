using Shared.Domain.Entities;

namespace Shared.Application.Interfaces.Repositories
{
    public interface IHistoricoRepository
    {
        Task<bool> AdicionarAsync(Historico historico);
        
        Task<IEnumerable<Historico>> ListarPorCodigoRegistroAsync(string codigoRegistro);
    }
}