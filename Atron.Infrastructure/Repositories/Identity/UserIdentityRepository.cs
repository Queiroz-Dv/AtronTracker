using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Shared.Extensions;
using Shared.Models.ApplicationModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.Identity
{
    public class UserIdentityRepository : IUserIdentityRepository
    {
        private AtronDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIdentityRepository(AtronDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
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

        public async Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.UserName == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.MinValue;

                _context.AppUsers.Update(user);
                var redefinido = await _context.SaveChangesAsync();
                return redefinido > 0;
            }

            return false;
        }

        public Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario)
        {
            return _context.AppUsers
                .Where(u => u.UserName == codigoUsuario)
                .Select(u => u.RefreshTokenExpireTime < DateTime.UtcNow)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(x => x.UserName == codigoUsuario);

            if (user is null)
                return false;


            user.Email = email;
            user.UserName = codigoUsuario;

            // Atualiza a senha apenas se foi informada uma nova
            if (senha.IsNullOrEmpty())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, senha);
                if (!passwordResult.Succeeded)
                    return false;
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            try
            {
                var applicationUser = new ApplicationUser
                {
                    UserName = codigoUsuario,
                    Email = email,
                    RefreshToken = null,
                    RefreshTokenExpireTime = DateTime.MinValue
                };

                var result = await _userManager.CreateAsync(applicationUser, senha);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(reg => reg.UserName == codigoUsuario);
            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);
                return result.Succeeded;
            }

            return false;
        }

        public async Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email)
        {
            var usuarioxistePorNome = await _userManager.Users.AnyAsync(reg => reg.UserName == codigoUsuario);
            var usuarioxistePorEmail = await _userManager.Users.AnyAsync(reg => reg.Email == email);

            // Retorna true se existir pelo nome ou pelo email
            return usuarioxistePorNome || usuarioxistePorEmail;
        }
    }
}