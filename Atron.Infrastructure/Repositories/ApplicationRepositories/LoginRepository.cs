using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IUsuarioIdentityRepository _userIdentityRepo;
        private readonly IPasswordHasher<UsuarioIdentity> _passwordHasher;

        public LoginRepository(IUsuarioIdentityRepository userIdentityRepo, IPasswordHasher<UsuarioIdentity> passwordHasher)
        {
            _userIdentityRepo = userIdentityRepo;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity)
        {
            var usuario = await _userIdentityRepo.ObterUsuarioIdentityPorCodigo(usuarioIdentity.Codigo);
            if (usuario != null)
            {
                var refreshTokenAtualizado = await _userIdentityRepo.AtualizarRefreshTokenUsuarioRepositoryAsync(
                    usuario.Codigo,
                    usuarioIdentity.RefreshToken,
                    usuarioIdentity.RefreshTokenExpireTime);

                if (refreshTokenAtualizado && !usuarioIdentity.Senha.IsNullOrEmpty())
                {
                    var autenticado = _passwordHasher.VerifyHashedPassword(usuario, usuario.SenhaHash, usuarioIdentity.Senha);
                    return autenticado == PasswordVerificationResult.Success;
                }

                return refreshTokenAtualizado;
            }

            return false;
        }

        public async Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha)
        {
            var usr = await _userIdentityRepo.ObterUsuarioIdentityPorCodigo(codigoDoUsuario);

            if (usr != null)
            {
                //usr = 

                //if (result.Succeeded)
                //{
                //    result = await _authManager.UserManager.AddPasswordAsync(usr, senha);
                //    return result.Succeeded;
                //}
            }

            return false;
        }

        public async Task Logout()
        {
            //await _authManager.SignInManager.SignOutAsync();
        }
    }
}