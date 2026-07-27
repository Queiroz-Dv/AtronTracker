using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Microsoft.Extensions.Logging;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IAuthManagerService _authManager;
        private readonly IUsuarioIdentityRepository _userIdentityRepo;
        private readonly ILogger<LoginRepository> _logger;

        public LoginRepository(
            IUsuarioIdentityRepository userIdentityRepo,
            IAuthManagerService authManagerContext,
            ILogger<LoginRepository> logger)
        {
            _userIdentityRepo = userIdentityRepo;
            _authManager = authManagerContext;
            _logger = logger;
        }

        public async Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity)
        {
            var usuario = await _authManager.UserManager.FindByNameAsync(usuarioIdentity.Codigo);
            if (usuario != null)
            {
                var refreshTokenAtualizado = await _userIdentityRepo.AtualizarRefreshTokenUsuarioRepositoryAsync(
                    usuario.UserName,
                    usuarioIdentity.RefreshToken,
                    usuarioIdentity.RefreshTokenExpireTime);

                if (refreshTokenAtualizado && !usuarioIdentity.Senha.IsNullOrEmpty())
                {
                    var signInResult = await _authManager.SignInManager.PasswordSignInAsync(usuarioIdentity.Codigo, usuarioIdentity.Senha, true, false);
                    return signInResult.Succeeded;
                }

                return refreshTokenAtualizado;
            }

            return false;
        }

        public async Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha)
        {
            var usr = await _authManager.UserManager.FindByNameAsync(codigoDoUsuario);

            if (usr != null)
            {
                var result = await _authManager.UserManager.RemovePasswordAsync(usr);

                if (result.Succeeded)
                {
                    result = await _authManager.UserManager.AddPasswordAsync(usr, senha);
                    return result.Succeeded;
                }
            }

            return false;
        }

        public async Task Logout()
        {
            await _authManager.SignInManager.SignOutAsync();
        }
    }
}
