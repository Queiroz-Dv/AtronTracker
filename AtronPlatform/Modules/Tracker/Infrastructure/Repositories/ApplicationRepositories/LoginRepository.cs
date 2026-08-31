using Domain.Entities;
using Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Domain.Entities.Identity;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginRepository : ILoginRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidarCredenciaisAsync(string codigoUsuario, string senha)
        {
            var usuario = await _userManager.FindByNameAsync(codigoUsuario);
            if (usuario is null || await _userManager.IsLockedOutAsync(usuario))
                return false;

            if (!usuario.LockoutEnabled)
            {
                usuario.LockoutEnabled = true;
                var lockoutHabilitado = await _userManager.UpdateAsync(usuario);
                if (!lockoutHabilitado.Succeeded)
                    return false;
            }

            if (!await _userManager.CheckPasswordAsync(usuario, senha))
            {
                await _userManager.AccessFailedAsync(usuario);
                return false;
            }

            var falhasRedefinidas = await _userManager.ResetAccessFailedCountAsync(usuario);
            return falhasRedefinidas.Succeeded;
        }

        public async Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha)
        {
            var usr = await _userManager.FindByNameAsync(codigoDoUsuario);

            if (usr != null)
            {
                var result = await _userManager.RemovePasswordAsync(usr);

                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(usr, senha);
                    return result.Succeeded;
                }
            }

            return false;
        }

    }
}
