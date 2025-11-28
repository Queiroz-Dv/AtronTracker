using Microsoft.EntityFrameworkCore;
using Shared.Application.Interfaces.Repositories;
using Shared.Domain.Entities;
using Shared.Infrastructure.Context;

namespace Shared.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly SharedDbContext _context;

        public AuditoriaRepository(SharedDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AdicionarAsync(Auditoria auditoria)
        {
            await _context.Auditorias.AddAsync(auditoria);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarAsync(Auditoria auditoria)
        {
            _context.Auditorias.Update(auditoria);
            return await _context.SaveChangesAsync() > 0;
        }       

        public async Task<Auditoria?> ObterPorContextoCodigoAsync(string contexto, string codigo)
        {
            return await _context.Auditorias
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Contexto == contexto && a.CodigoRegistro == codigo);
        }
    }
}