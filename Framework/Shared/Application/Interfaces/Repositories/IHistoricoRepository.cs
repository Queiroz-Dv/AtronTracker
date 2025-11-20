using Shared.Domain.Entities;

namespace Shared.Application.Interfaces.Repositories
{
    public interface IHistoricoRepository
    {
        Task<bool> AdicionarAsync(Historico historico);

        // Retorna a lista ordenada pela sequence, ideal para timeline
        Task<IEnumerable<Historico>> ListarPorCodigoRegistroAsync(string codigoRegistro);
    }
}