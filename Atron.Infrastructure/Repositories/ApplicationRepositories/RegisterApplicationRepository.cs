using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models.ApplicationModels;
using System;
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
            try
            {
                ApplicationUser applicationUser = CreateUser(register);
                var result = await _userManager.CreateAsync(applicationUser, register.Password);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAccountAsync(ApiRegister register)
        {
            var user = await _userManager.FindByNameAsync(register.UserName);

            if (user is null)
                return false;


            user.Email = register.Email;
            user.UserName = register.UserName;

            // Atualiza a senha apenas se foi informada uma nova
            if (!string.IsNullOrWhiteSpace(register.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, register.Password);
                if (!passwordResult.Succeeded)
                    return false;
            }


            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;

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

        public async Task<bool> UserExists(ApiRegister register)
        {
            var userExistsByName = await _userManager.Users.AnyAsync(reg => reg.UserName == register.UserName);
            var userExistsByEmail = await _userManager.Users.AnyAsync(reg => reg.Email == register.Email);

            return userExistsByName || userExistsByEmail;
        }

        public async Task<bool> UserExistsByUserCode(string code)
        {
            return await _userManager.Users.AnyAsync(reg => reg.UserName == code);
        }

        public async Task<bool> UserExistsByEmail(string email)
        {
            return await _userManager.Users.AnyAsync(reg => reg.Email == email);
        }
    }
}