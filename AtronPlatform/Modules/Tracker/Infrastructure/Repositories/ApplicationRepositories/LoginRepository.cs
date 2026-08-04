using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly IAuthManagerService _authManager;
        public LoginRepository(IAuthManagerService authManagerContext)
        {
            _authManager = authManagerContext;
        }

        public async Task<bool> ValidarCredenciaisAsync(string codigoUsuario, string senha)
        {
            var usuario = await _authManager.UserManager.FindByNameAsync(codigoUsuario);
            if (usuario is null || await _authManager.UserManager.IsLockedOutAsync(usuario))
                return false;

            if (!usuario.LockoutEnabled)
            {
                usuario.LockoutEnabled = true;
                var lockoutHabilitado = await _authManager.UserManager.UpdateAsync(usuario);
                if (!lockoutHabilitado.Succeeded)
                    return false;
            }

            if (!await _authManager.UserManager.CheckPasswordAsync(usuario, senha))
            {
                await _authManager.UserManager.AccessFailedAsync(usuario);
                return false;
            }

            var falhasRedefinidas = await _authManager.UserManager.ResetAccessFailedCountAsync(usuario);
            return falhasRedefinidas.Succeeded;
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

    }
}
