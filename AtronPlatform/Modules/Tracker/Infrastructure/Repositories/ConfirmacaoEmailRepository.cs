using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class ConfirmacaoEmailRepository : Repository<ConfirmacaoEmail>, IConfirmacaoEmailRepository
    {
        private readonly AtronDbContext _context;

        public ConfirmacaoEmailRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> GravarOuSubstituirAsync(ConfirmacaoEmail confirmacaoEmail)
        {
            confirmacaoEmail.CriadoEm = SemTimezone(confirmacaoEmail.CriadoEm);
            confirmacaoEmail.ExpiraEm = SemTimezone(confirmacaoEmail.ExpiraEm);

            var pendentes = await _context.ConfirmacoesEmail
                .Where(cfm => cfm.UsuarioCodigo == confirmacaoEmail.UsuarioCodigo && cfm.ConfirmadoEm == null)
                .ToListAsync();

            if (pendentes.Any())
            {
                _context.ConfirmacoesEmail.RemoveRange(pendentes);
            }

            await _context.ConfirmacoesEmail.AddAsync(confirmacaoEmail);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ConfirmacaoEmail> ObterAtivaPorUsuarioAsync(string usuarioCodigo)
        {
            var agora = SemTimezone(DateTime.UtcNow);

            return await _context.ConfirmacoesEmail
                .AsNoTracking()
                .Where(cfm =>
                    cfm.UsuarioCodigo == usuarioCodigo &&
                    cfm.ConfirmadoEm == null &&
                    cfm.ExpiraEm >= agora)
                .OrderByDescending(cfm => cfm.CriadoEm)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> MarcarConfirmadaAsync(int id)
        {
            var confirmacao = await _context.ConfirmacoesEmail
                .FirstOrDefaultAsync(cfm => cfm.Id == id);

            if (confirmacao is null)
            {
                return false;
            }

            confirmacao.ConfirmadoEm = SemTimezone(DateTime.UtcNow);
            return await _context.SaveChangesAsync() > 0;
        }

        private static DateTime SemTimezone(DateTime data)
            => DateTime.SpecifyKind(data, DateTimeKind.Unspecified);
    }
}
