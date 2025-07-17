using Atron.Domain.Entities;
using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
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
    public class UserIdentityRepository : Repository<UsuarioIdentity>, IUserIdentityRepository
    {
        private AtronDbContext atronDbContext;

        private readonly ILiteUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserIdentityRepository(AtronDbContext context,
                                      UserManager<ApplicationUser> userManager,
                                      ILiteDbContext liteDbContext,
                                      ILiteUnitOfWork uow) : base(context, liteDbContext)
        {
            atronDbContext = context;
            _userManager = userManager;
            _uow = uow;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
        {
            var user = await _liteContext.Users.FindOneAsync(u => u.UserName == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireTime = refreshTokenExpireTime;

                _uow.BeginTransaction();
                try
                {
                    var atualizado = await _liteContext.Users.UpdateAsync(user);

                    if (atualizado)
                    {
                        _uow.Commit();
                        return atualizado;
                    }
                }
                catch
                {
                    _uow.Rollback();
                    throw;
                }
            }

            return false;
        }

        public async Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken)
        {
            return await _liteContext.Users.AnyAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario)
        {
            var usuario = await _liteContext.Users.FindOneAsync(u => u.UserName == codigoUsuario);
            return usuario?.RefreshToken;
        }

        public async Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
        {
            var user = await _liteContext.Users.FindOneAsync(u => u.UserName == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.MinValue;

                _uow.BeginTransaction();
                try
                {
                    var atualizado = await _liteContext.Users.UpdateAsync(user);

                    if (atualizado)
                    {
                        _uow.Commit();
                        return atualizado;
                    }

                    return atualizado;
                }
                catch
                {
                    _uow.Rollback();
                    throw;
                }
            }

            return false;
        }

        public async Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario)
        {
            var usuario = await _liteContext.Users.FindOneAsync(u => u.UserName == codigoUsuario);
            return usuario.RefreshTokenExpireTime < DateTime.UtcNow;
        }

        public async Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            var user = await atronDbContext.AppUsers.FirstOrDefaultAsync(x => x.UserName == codigoUsuario);

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

            _uow.BeginTransaction();
            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _uow.Commit();
                }

                return result.Succeeded;

            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            _uow.BeginTransaction();
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

                if (result.Succeeded)
                {
                    _uow.Commit();
                    return result.Succeeded;
                }
                else
                {
                    _uow.Rollback();
                    return false;
                }
            }
            catch
            {
                _uow.Rollback();
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

            var usuarioxistePorNome = await _liteContext.Users.AnyAsync(reg => reg.UserName == codigoUsuario);
            var usuarioxistePorEmail = await _liteContext.Users.AnyAsync(reg => reg.Email == email);

            // Retorna true se existir pelo nome ou pelo email
            return usuarioxistePorNome || usuarioxistePorEmail;
        }
    }
}