using Microsoft.EntityFrameworkCore;
using AtronAuditoria.Application.Interfaces.Repositories;
using AtronAuditoria.Domain.Entities;
using AtronAuditoria.Infrastructure.Context;

namespace AtronAuditoria.Repositories
{
    public class AuditoriaRepository : IAuditoriaRepository
    {
        private readonly AtronAuditoriaContext _context;

        public AuditoriaRepository(AtronAuditoriaContext context)
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
