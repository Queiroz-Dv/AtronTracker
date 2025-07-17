using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Models.ApplicationModels;
using System;
using System.Threading.Tasks;

namespace Atron.Infrastructure.Repositories.ApplicationRepositories
{
    public class RegisterApplicationRepository : Repository<UsuarioRegistro>, IRegisterApplicationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILiteUnitOfWork _uow;

        public RegisterApplicationRepository(AtronDbContext context,
                                             UserManager<ApplicationUser> userManager,
                                             ILiteDbContext liteDbContext,
                                             ILiteUnitOfWork uow) : base(context, liteDbContext)
        {
            _userManager = userManager;
            _uow = uow;
        }

        public async Task<bool> RegisterUserAccountAsync(UsuarioRegistro register)
        {
            _uow.BeginTransaction();
            try
            {
                ApplicationUser applicationUser = new()
                {
                    UserName = register.UserName,
                    Email = register.Email
                };

                var result = await _userManager.CreateAsync(applicationUser, register.Password);
                if (result.Succeeded)
                {
                    _uow.Commit();
                    return result.Succeeded;
                }
                else
                {
                    _uow.Rollback();
                    return false;
                }
            }
            catch
            {
                _uow.Rollback();
                return false;
            }
        }

        public async Task<bool> UpdateUserAccountAsync(UsuarioRegistro register)
        {
            var user = await _userManager.FindByNameAsync(register.UserName);

            if (user is null)
                return false;

            // Informações principais do usuário do identity
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

            _uow.BeginTransaction();
            try
            {

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _uow.Commit();
                    return result.Succeeded;
                }
                else
                {
                    _uow.Rollback();
                    return false;
                }
            }
            catch
            {
                _uow.Rollback();
                return false;
            }
        }

        public async Task DeleteAccountUserAsync(string userName)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(reg => reg.UserName == userName);

            _uow.BeginTransaction();
            try
            {
                var result  = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _uow.Commit();
                }
                else
                {
                    _uow.Rollback();
                }   

            }
            catch
            {
                _uow.Rollback();
                throw;
            }
        }

        public async Task<bool> UserExists(UsuarioRegistro register)
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