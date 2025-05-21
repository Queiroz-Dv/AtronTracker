using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Models.ApplicationModels;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class LoginApplicationRepository : ILoginApplicationRepository
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginApplicationRepository(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> AuthenticateUserLoginAsync(ApiLogin login)
        {
            var authenticated = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, login.Remember, false);

            return authenticated.Succeeded;
        }

        public async Task<bool> ConfigPasswordAsync(string codigoDoUsuario, ApiLogin model)
        {
            var usr = await _userManager.FindByNameAsync(codigoDoUsuario);
            if (usr != null)
            {
                var result = await _userManager.RemovePasswordAsync(usr);

                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(usr, model.Password);
                    return result.Succeeded;
                }
            }

            return false;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}