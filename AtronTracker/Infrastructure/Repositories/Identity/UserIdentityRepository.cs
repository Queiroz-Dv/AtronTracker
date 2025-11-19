using Domain.ApiEntities;
using Domain.Entities;
using Domain.Interfaces.Identity;
using AtronTracker.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Shared.Domain.Entities.Identity;

namespace Infrastructure.Repositories.Identity
{
    public class UserIdentityRepository : IUsuarioIdentityRepository
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
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserName == codigoUsuario);

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
                var atualizado = await _context.SaveChangesAsync();
                return atualizado > 0;
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
            var user = await _userManager.FindByNameAsync(codigoUsuario);

            if (user is null)
                return false;

            user.Email = email;
            user.UserName = codigoUsuario;

            // Atualiza a senha apenas se foi informada uma nova
            if (!senha.IsNullOrEmpty())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultadoDaSenha = await _userManager.ResetPasswordAsync(user, token, senha);
                if (!resultadoDaSenha.Succeeded)
                    return false;
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        private static ApplicationUser CreateUser(ApiRegister register)
        {
            return new ApplicationUser()
            {
                UserName = register.UserName,
                Email = register.Email
            };
        }

        public async Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            try
            {
                var applicationUser = CreateUser(new ApiRegister(codigoUsuario, email, senha, senha));
                var result = await _userManager.CreateAsync(applicationUser, senha);
                return result.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(reg => reg.UserName == codigoUsuario);
            var usuarioDeletado = await _userManager.DeleteAsync(user);
            return usuarioDeletado.Succeeded;
        }

        public async Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == codigoUsuario && x.Email == email);
        }

        public async Task<UsuarioIdentity> ObterUsuarioIdentityPorCodigo(string codigoUsuario)
        {
            var applicationUsuario = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == codigoUsuario);

            var usuarioIdentity = new UsuarioIdentity()
            {
                Codigo = applicationUsuario.UserName,
                Email = applicationUsuario.Email,
            };

            return usuarioIdentity;
        }
    }
}