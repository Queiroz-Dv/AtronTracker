using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces.Repositories;
using Shared.Domain.Entities;
using Shared.Infrastructure.Context;

namespace Shared.Repositories
{
    public class HistoricoRepository : IHistoricoRepository
    {
        private readonly SharedDbContext _context;

        public HistoricoRepository(SharedDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarAsync(Historico historico)
        {
            await _context.Historicos.AddAsync(historico);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Historico>> ListarPorCodigoRegistroAsync(string codigoRegistro)
        {
            return await _context.Historicos
                .AsNoTracking()
                .Where(h => h.CodigoRegistro == codigoRegistro)
                .OrderByDescending(h => h.CodigoHistorico)
                .ToListAsync();
        }
    }
}