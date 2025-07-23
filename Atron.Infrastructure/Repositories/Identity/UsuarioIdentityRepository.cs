using Atron.Domain.Entities;
using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.Identity
{
    public class UsuarioIdentityRepository : IUsuarioIdentityRepository
    {
        private ILiteDbContext context;
        private ILiteUnitOfWork unitOfWork;
        private IServiceAccessor serviceAccessor;
        private readonly IPasswordHasher<UsuarioIdentity> _hasher;

        public UsuarioIdentityRepository(ILiteDbContext context,
                                         ILiteUnitOfWork unitOfWork,
                                         IServiceAccessor serviceAccessor,
                                         IPasswordHasher<UsuarioIdentity> hasher)
        {
            this.context = context;
            this.unitOfWork = unitOfWork;
            this.serviceAccessor = serviceAccessor;
            _hasher = hasher;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioIdentity = await context.UsuarioIdentity.FindOneAsync(u => u.Codigo == codigoUsuario);

                if (usuarioIdentity != null)
                {
                    usuarioIdentity.RefreshToken = refreshToken;
                    usuarioIdentity.RefreshTokenExpireTime = refreshTokenExpireTime;
                    var atualizado = await context.UsuarioIdentity.UpdateAsync(usuarioIdentity);
                    unitOfWork.Commit();
                    return atualizado;
                }

                serviceAccessor.ObterService<MessageModel>().AdicionarErro("Código de usuário não encontrado");
                return false;

            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken)
        {
            return await context.UsuarioIdentity.AnyAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario)
        {
            var usuario = await context.UsuarioIdentity.FindOneAsync(u => u.Codigo == codigoUsuario);
            return usuario?.RefreshToken;
        }

        public async Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioIdentity = await context.UsuarioIdentity.FindOneAsync(u => u.Codigo == codigoUsuario);

                if (usuarioIdentity != null)
                {
                    usuarioIdentity.RefreshToken = null;
                    usuarioIdentity.RefreshTokenExpireTime = DateTime.MinValue;

                    var atualizado = await context.UsuarioIdentity.UpdateAsync(usuarioIdentity);
                    unitOfWork.Commit();
                    return atualizado;
                }

                serviceAccessor.ObterService<MessageModel>().AdicionarErro("Código de usuário não encontrado");
                return false;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario)
        {
            var usuario = await context.UsuarioIdentity.FindOneAsync(u => u.Codigo == codigoUsuario);
            return usuario.RefreshTokenExpireTime < DateTime.UtcNow;
        }

        public async Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioIdentity = await context.UsuarioIdentity.FindOneAsync(x => x.Codigo == codigoUsuario);

                if (usuarioIdentity is null)
                    return false;

                usuarioIdentity.Email = email;
                usuarioIdentity.Codigo = codigoUsuario;

                // Atualiza a senha apenas se foi informada uma nova
                if (senha.IsNullOrEmpty())
                {
                    var senhaHashed = _hasher.HashPassword(usuarioIdentity, senha);
                    usuarioIdentity.SenhaHash = senhaHashed;
                }

                var atualizado = await context.UsuarioIdentity.UpdateAsync(usuarioIdentity);
                unitOfWork.Commit();
                return atualizado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var senhaHashed = _hasher.HashPassword(null, senha);
                var applicationUser = new UsuarioIdentity
                {
                    Email = email,
                    Codigo = codigoUsuario,
                    SenhaHash = senhaHashed
                };
                var gravado = await context.UsuarioIdentity.InsertAsync(applicationUser);

                unitOfWork.Commit();

                return gravado > 0;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario)
        {
            try
            {
                unitOfWork.BeginTransaction();
                var usuarioIdentity = await context.UsuarioIdentity.FindOneAsync(reg => reg.Codigo == codigoUsuario);

                var deletado = await context.UsuarioIdentity.DeleteAsync(usuarioIdentity.Id);
                unitOfWork.Commit();

                return deletado;
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();
                serviceAccessor.ObterService<MessageModel>().AdicionarErro(ex.Message);
                return false;
            }
        }

        public async Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email)
        {

            var usuarioxistePorNome = await context.UsuarioIdentity.AnyAsync(reg => reg.Codigo == codigoUsuario);
            var usuarioxistePorEmail = await context.UsuarioIdentity.AnyAsync(reg => reg.Email == email);

            // Retorna true se existir pelo nome ou pelo email
            return usuarioxistePorNome || usuarioxistePorEmail;
        }

        public async Task<UsuarioIdentity> ObterUsuarioIdentityPorCodigo(string codigoUsuario)
        {
            return await context.UsuarioIdentity.FindOneAsync(u => u.Codigo == codigoUsuario);
        }
    }
}