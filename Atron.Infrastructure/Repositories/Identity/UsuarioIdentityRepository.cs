using Atron.Domain.Entities;
using Atron.Domain.Interfaces.Identity;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.Identity
{
    public class UsuarioIdentityRepository : Repository<UsuarioIdentity>, IUsuarioIdentityRepository
    {
        public IDataSet<UsuarioIdentity> Users => _facade.LiteDbContext.UsuarioIdentity;
        private readonly IPasswordHasher<UsuarioIdentity> _hasher;

        public UsuarioIdentityRepository(ILiteFacade liteFacade, IServiceAccessor serviceAccessor, IPasswordHasher<UsuarioIdentity> hasher)
            : base(liteFacade, serviceAccessor)
        {
            _hasher = hasher;
        }

        public async Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime)
        {
            var user = await Users.FindOneAsync(u => u.Codigo == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpireTime = refreshTokenExpireTime;
                await Users.UpdateAsync(user);
            }

            return false;
        }

        public async Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken)
        {
            return await Users.AnyAsync(u => u.RefreshToken == refreshToken);
        }

        public async Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario)
        {
            var usuario = await Users.FindOneAsync(u => u.Codigo == codigoUsuario);
            return usuario?.RefreshToken;
        }

        public async Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario)
        {
            var user = await Users.FindOneAsync(u => u.Codigo == codigoUsuario);

            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpireTime = DateTime.MinValue;

                return await Users.UpdateAsync(user);
            }

            return false;
        }

        public async Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario)
        {
            var usuario = await Users.FindOneAsync(u => u.Codigo == codigoUsuario);
            return usuario.RefreshTokenExpireTime < DateTime.UtcNow;
        }

        public async Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha)
        {
            var user = await Users.FindOneAsync(x => x.Codigo == codigoUsuario);

            if (user is null)
                return false;


            user.Email = email;
            user.Codigo = codigoUsuario;

            // Atualiza a senha apenas se foi informada uma nova
            if (senha.IsNullOrEmpty())
            {
                var senhaHashed = _hasher.HashPassword(user, senha);
                user.SenhaHash = senhaHashed;
            }

            return await Users.UpdateAsync(user);
        }

        public async Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha)
        {

            var senhaHashed = _hasher.HashPassword(null, senha);
            var applicationUser = new UsuarioIdentity
            {
                Email = email,
                Codigo = codigoUsuario,
                SenhaHash = senhaHashed
            };
            var gravado = await Users.InsertAsync(applicationUser);
            return gravado > 0;
        }

        public async Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario)
        {
            var user = await Users.FindOneAsync(reg => reg.Codigo == codigoUsuario);

            return await Users.DeleteAsync(user.Id);
        }

        public async Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email)
        {

            var usuarioxistePorNome = await Users.AnyAsync(reg => reg.Codigo == codigoUsuario);
            var usuarioxistePorEmail = await Users.AnyAsync(reg => reg.Email == email);

            // Retorna true se existir pelo nome ou pelo email
            return usuarioxistePorNome || usuarioxistePorEmail;
        }
    }
}