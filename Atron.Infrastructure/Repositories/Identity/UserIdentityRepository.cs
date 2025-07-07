using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.Identity
{
    public class UserIdentityRepository : IUserIdentityRepository
    {
        private AtronDbContext _context;

        public UserIdentityRepository(AtronDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
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

        public async Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken)
        {
            return await _context.AppUsers.AnyAsync(u => u.RefreshToken == refreshToken);                
        }

        public Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario)
        {
            return _context.AppUsers
                .Where(u => u.UserName == codigoUsuario)
                .Select(u => u.RefreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
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

        public Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario)
        {
            return _context.AppUsers
                .Where(u => u.UserName == codigoUsuario)
                .Select(u => u.RefreshTokenExpireTime < DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }
    }
}