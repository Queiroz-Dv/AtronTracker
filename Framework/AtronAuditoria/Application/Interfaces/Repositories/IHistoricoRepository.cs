using AtronAuditoria.Domain.Entities;

namespace AtronAuditoria.Application.Interfaces.Repositories
{
    public interface IHistoricoRepository
    {
        Task<bool> AdicionarAsync(Historico historico);              

        Task<IEnumerable<Historico>> ListarPorContextoCodigoAsync(string contexto, string codigo);
    }
}
