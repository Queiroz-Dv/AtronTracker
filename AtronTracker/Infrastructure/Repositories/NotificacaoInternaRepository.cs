using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class NotificacaoInternaRepository : Repository<NotificacaoInterna>, INotificacaoInternaRepository
    {
        private readonly AtronDbContext _context;

        public NotificacaoInternaRepository(AtronDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<NotificacaoInterna>> ObterPorUsuarioAsync(int usuarioId, string usuarioCodigo)
        {
            return await _context.Set<NotificacaoInterna>()
                .AsNoTracking()
                .Where(ntf => ntf.UsuarioId == usuarioId && ntf.UsuarioCodigo == usuarioCodigo)
                .OrderBy(ntf => ntf.Lida)
                .ThenByDescending(ntf => ntf.DataCriacao)
                .ToListAsync();
        }

        public async Task<NotificacaoInterna> ObterPorIdEUsuarioAsync(int id, int usuarioId, string usuarioCodigo)
        {
            return await _context.Set<NotificacaoInterna>()
                .AsNoTracking()
                .FirstOrDefaultAsync(ntf =>
                    ntf.Id == id &&
                    ntf.UsuarioId == usuarioId &&
                    ntf.UsuarioCodigo == usuarioCodigo);
        }

        public async Task<bool> CriarAsync(NotificacaoInterna notificacao)
        {
            await _context.Set<NotificacaoInterna>().AddAsync(notificacao);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> MarcarComoLidaAsync(int id, int usuarioId, string usuarioCodigo)
        {
            var notificacao = await _context.Set<NotificacaoInterna>()
                .FirstOrDefaultAsync(ntf =>
                    ntf.Id == id &&
                    ntf.UsuarioId == usuarioId &&
                    ntf.UsuarioCodigo == usuarioCodigo);

            if (notificacao is null)
            {
                return false;
            }

            if (!notificacao.Lida)
            {
                notificacao.Lida = true;
                notificacao.DataLeitura = DateTime.Now;
                return await _context.SaveChangesAsync() > 0;
            }

            return true;
        }

        public async Task<bool> MarcarTodasComoLidasAsync(int usuarioId, string usuarioCodigo)
        {
            var notificacoes = await _context.Set<NotificacaoInterna>()
                .Where(ntf =>
                    ntf.UsuarioId == usuarioId &&
                    ntf.UsuarioCodigo == usuarioCodigo &&
                    !ntf.Lida)
                .ToListAsync();

            if (!notificacoes.Any())
            {
                return true;
            }

            var dataLeitura = DateTime.Now;
            foreach (var notificacao in notificacoes)
            {
                notificacao.Lida = true;
                notificacao.DataLeitura = dataLeitura;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
