using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Models.ApplicationModels;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class RegisterApplicationRepository : IRegisterApplicationRepository
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public RegisterApplicationRepository(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<bool> RegisterUserAccountAsync(Register register)
        {
            var applicationUser = new ApplicationUser()
            {
                UserName = register.UserName,
                Email = register.Email                
            };

            var result = await _userManager.CreateAsync(applicationUser, register.Password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, false);
            }

            return result.Succeeded;
        }
    }
}