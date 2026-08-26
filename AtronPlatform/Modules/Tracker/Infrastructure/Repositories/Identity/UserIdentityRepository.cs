using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Domain.Entities.Identity;
using Shared.Extensions;

namespace Infrastructure.Repositories.Identity
{
    public class UserIdentityRepository(AtronDbContext context, UserManager<ApplicationUser> userManager) : IUsuarioIdentityRepository
    {
        private readonly AtronDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<bool> DesativarContaAsync(string codigoUsuario)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return false;

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> ReativarContaAsync(string codigoUsuario)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return false;

            user.LockoutEnd = null;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshTokenHash, DateTime refreshTokenExpireTime)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserName == codigoUsuario);
            if (user is null) return false;

            user.RefreshToken = refreshTokenHash;
            // É necessário fazer dessa forma por causa do PostgreSQL, que não aceita DateTimeKind.Utc, então definimos como Unspecified
            user.RefreshTokenExpireTime = DateTime.SpecifyKind(refreshTokenExpireTime, DateTimeKind.Unspecified);

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> RefreshTokenExisteRepositoryAsync(string refreshTokenHash)
        {
            return await _context.AppUsers.AnyAsync(u => u.RefreshToken == refreshTokenHash);
        }

        public async Task<SessaoRefreshToken> ObterSessaoRefreshTokenRepositoryAsync(string refreshTokenHash)
        {
            return await _context.AppUsers
                .Where(u => u.RefreshToken == refreshTokenHash && u.RefreshTokenExpireTime.HasValue)
                .Select(u => new SessaoRefreshToken(u.UserName, u.RefreshTokenExpireTime.Value))
                .FirstOrDefaultAsync();
        }

        public async Task<bool> RotacionarRefreshTokenRepositoryAsync(RotacaoRefreshTokenHash rotacao)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u =>
                u.UserName == rotacao.UsuarioCodigo && u.RefreshToken == rotacao.HashAtual);

            if (user is null) return false;

            user.RefreshToken = rotacao.NovoHash;
            user.RefreshTokenExpireTime = DateTime.SpecifyKind(rotacao.NovaExpiracao, DateTimeKind.Unspecified);

            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public async Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
        {
            var user = await _context.AppUsers.FirstOrDefaultAsync(u => u.UserName == codigoUsuario);
            if (user is null) return false;
            if (user.RefreshToken is null) return true;

            user.RefreshToken = null;
            user.RefreshTokenExpireTime = null;
            _context.AppUsers.Update(user);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return false;

            user.Email = email;
            user.UserName = codigoUsuario;

            if (!senha.IsNullOrEmpty())
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resultadoDaSenha = await _userManager.ResetPasswordAsync(user, token, senha);
                if (!resultadoDaSenha.Succeeded) return false;
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
                    Email = email
                };
                var result = await _userManager.CreateAsync(applicationUser, senha);
                return result.Succeeded;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName == codigoUsuario);
            if (user is null) return false;
            return (await _userManager.DeleteAsync(user)).Succeeded;
        }

        public async Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == codigoUsuario || x.Email == email);
        }

        public async Task<UsuarioIdentity> ObterUsuarioIdentityPorCodigo(string codigoUsuario)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == codigoUsuario);
            if (user is null) return null;

            return new UsuarioIdentity
            {
                Codigo = user.UserName,
                Email = user.Email
            };
        }

        public async Task<string> GerarTokenConfirmacaoEmailAsync(string codigoUsuario)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return null;
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }

        public async Task<bool> ConfirmarEmailAsync(string codigoUsuario, string token)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return false;
            return (await _userManager.ConfirmEmailAsync(user, token)).Succeeded;
        }

        public async Task<string> GerarTokenAlteracaoEmailAsync(string codigoUsuario, string emailNovo)
        {
            var usuario = await _userManager.FindByNameAsync(codigoUsuario);
            if (usuario == null) return string.Empty;

            return await _userManager.GenerateChangeEmailTokenAsync(usuario, emailNovo);
        }

        public async Task<bool> ConfirmarAlteracaoEmailAsync(string codigoUsuario, string emailNovo, string token)
        {
            var usuario = await _userManager.FindByNameAsync(codigoUsuario);
            if (usuario == null) return false;

            var resultado = await _userManager.ChangeEmailAsync(usuario, emailNovo, token);

            if (resultado.Succeeded)
            {
                // Mantém o UserName sincronizado com o e-mail no Identity
                usuario.Email = emailNovo;
                usuario.NormalizedEmail = emailNovo.ToUpperInvariant();
                await _userManager.UpdateAsync(usuario);
            }

            return resultado.Succeeded;
        }

        public async Task<string> GerarTokenRecuperacaoSenhaAsync(string codigoUsuario)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return null;
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<bool> RedefinirSenhaAsync(string codigoUsuario, string token, string novaSenha)
        {
            var user = await _userManager.FindByNameAsync(codigoUsuario);
            if (user is null) return false;
            var result = await _userManager.ResetPasswordAsync(user, token, novaSenha);
            return result.Succeeded;
        }

        public async Task<bool> EmailConfirmadoAsync(string codigoUsuario)
        {
            var usuario = await _userManager.FindByNameAsync(codigoUsuario);
            if (usuario == null) return false;

            return await _userManager.IsEmailConfirmedAsync(usuario);
        }
    }
}
