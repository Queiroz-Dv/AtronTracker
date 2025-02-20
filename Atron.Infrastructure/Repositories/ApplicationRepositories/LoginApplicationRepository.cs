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

        public LoginApplicationRepository(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> AuthenticateUserLoginAsync(ApiLogin login)
        {
            var authenticated = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, true, false);

            return authenticated.Succeeded;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();            
        }
    }
}