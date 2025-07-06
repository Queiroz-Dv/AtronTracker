using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private AtronDbContext _context;

        public AppUserRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarRefreshTokenUsuario(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserName == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireTime = refreshTokenExpireTime;
                _context.AppUsers.Update(user);
                var gravado = await _context.SaveChangesAsync();
                return gravado > 0;
            }

            return false;
        }

        public async Task<bool> RefreshTokenExiste(string refreshToken)
        {
            return await _context.AppUsers.AnyAsync(u => u.RefreshToken == refreshToken);                
        }

        public Task<string> ObterRefreshTokenPorCodigoUsuario(string codigoUsuario)
        {
            return _context.AppUsers
                .Where(u => u.UserName == codigoUsuario)
                .Select(u => u.RefreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task RedefinirRefreshToken(string codigoUsuario)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserName == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.MinValue;

                _context.AppUsers.Update(user);
                await _context.SaveChangesAsync();
            }
        }

        public Task<bool> RefreshTokenExpirado(string codigoUsuario)
        {
            return _context.AppUsers
                .Where(u => u.UserName == codigoUsuario)
                .Select(u => u.RefreshTokenExpireTime < DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }
    }
}