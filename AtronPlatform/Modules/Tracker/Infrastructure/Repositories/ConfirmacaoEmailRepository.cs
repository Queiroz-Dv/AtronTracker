using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;

namespace Infrastructure.Repositories
{
    public class ConfirmacaoEmailRepository(AtronDbContext context) : IConfirmacaoEmailRepository
    {
        private readonly AtronDbContext _context = context;

        public async Task<bool> GravarOuSubstituirAsync(ConfirmacaoEmail confirmacaoEmail)
        {
            confirmacaoEmail.CriadoEm = confirmacaoEmail.CriadoEm.SemTimezone();
            confirmacaoEmail.ExpiraEm = confirmacaoEmail.ExpiraEm.SemTimezone();

            var pendentes = await _context.ConfirmacoesEmail
                .Where(cfm => cfm.UsuarioCodigo == confirmacaoEmail.UsuarioCodigo && cfm.ConfirmadoEm == null)
                .ToListAsync();

            if (pendentes.Count != 0)
            {
                _context.ConfirmacoesEmail.RemoveRange(pendentes);
            }

            await _context.ConfirmacoesEmail.AddAsync(confirmacaoEmail);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<ConfirmacaoEmail> ObterAtivaPorUsuarioAsync(string usuarioCodigo)
        {
            var agora = DateTime.UtcNow.SemTimezone();

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
            var confirmacao = await _context.ConfirmacoesEmail.FirstOrDefaultAsync(cfm => cfm.Id == id);

            if (confirmacao is null)
                return false;


            confirmacao.ConfirmadoEm = DateTime.UtcNow.SemTimezone();
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task RegistrarTentativaFalhaAsync(int id)
        {
            var confirmacao = await _context.ConfirmacoesEmail
                .FirstOrDefaultAsync(item => item.Id == id);

            if (confirmacao is null)
                return;

            confirmacao.TentativasFalhas++;
            await _context.SaveChangesAsync();
        }
    }
}