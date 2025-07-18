using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.Identity;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginRepository : ILoginRepository
    {

        private readonly IUsuarioIdentityRepository _userIdentityRepo;

        public LoginRepository(IUsuarioIdentityRepository userIdentityRepo)
        {
            _userIdentityRepo = userIdentityRepo;
        }

        public async Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity)
        {
            var usuario = await _userIdentityRepo.ObterPorCodigoRepositoryAsync(usuarioIdentity.Codigo);
            if (usuario != null)
            {
                var refreshTokenAtualizado = await _userIdentityRepo.AtualizarRefreshTokenUsuarioRepositoryAsync(
                    usuario.Codigo,
                    usuarioIdentity.RefreshToken,
                    usuarioIdentity.RefreshTokenExpireTime);

                if (refreshTokenAtualizado && !usuarioIdentity.Senha.IsNullOrEmpty())
                {
                    //var signInResult = await _authManager.SignInManager.PasswordSignInAsync(usuarioIdentity.Codigo, usuarioIdentity.Senha, true, false);
                    //return signInResult.Succeeded;
                }

                return refreshTokenAtualizado;
            }

            return false;
        }

        public async Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha)
        {
            var usr = await _userIdentityRepo.ObterPorCodigoRepositoryAsync(codigoDoUsuario);

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