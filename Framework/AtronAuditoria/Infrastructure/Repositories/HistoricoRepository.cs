using Microsoft.EntityFrameworkCore;
using AtronAuditoria.Application.Interfaces.Repositories;
using AtronAuditoria.Domain.Entities;
using AtronAuditoria.Infrastructure.Context;

namespace AtronAuditoria.Repositories
{
    public class HistoricoRepository : IHistoricoRepository
    {
        private readonly AtronAuditoriaContext _context;

        public HistoricoRepository(AtronAuditoriaContext context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarAsync(Historico historico)
        {
            await _context.Historicos.AddAsync(historico);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Historico>> ListarPorContextoCodigoAsync(string contexto, string codigo)
        {
            return await _context.Historicos
                .AsNoTracking()
                .Where(h => h.Contexto == contexto && h.CodigoRegistro == codigo)
                .OrderByDescending(h => h.CodigoHistorico)
                .ToListAsync();
        }
    }
}
