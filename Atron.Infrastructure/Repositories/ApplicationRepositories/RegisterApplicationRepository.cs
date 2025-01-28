using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models.ApplicationModels;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class RegisterApplicationRepository : IRegisterApplicationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterApplicationRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> RegisterUserAccountAsync(ApiRegister register)
        {
            ApplicationUser applicationUser = CreateUser(register);

            try
            {
                var result = await _userManager.CreateAsync(applicationUser, register.Password);

                return result.Succeeded;
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> UpdateUserAccountAsync(ApiRegister register)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(reg => reg.UserName == register.UserName);
            if (user is not null)
            {
                user.Email = register.Email;

                var result = await _userManager.UpdateAsync(user);

                return result.Succeeded;
            }

            return false;
        }

        private static ApplicationUser CreateUser(ApiRegister register)
        {
            return new ApplicationUser()
            {
                UserName = register.UserName,
                Email = register.Email
            };
        }

        public async Task DeleteAccountUserAsync(string userName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(reg => reg.UserName == userName);
            await _userManager.DeleteAsync(user);
        }
    }
}