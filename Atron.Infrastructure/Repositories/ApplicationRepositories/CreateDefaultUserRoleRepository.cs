using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Models.ApplicationModels;
using System;

namespace Atron.Tracker.Infrastructure.Repositories.ApplicationRepositories
{
    public class CreateDefaultUserRoleRepository : ICreateDefaultUserRoleRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateDefaultUserRoleRepository(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void CreateDefaultUsers()
        {
            // Use
            if (_userManager.FindByEmailAsync("atronUser@email.com").Result == null)
            {
                var applicationUser = new ApplicationUser();
                applicationUser.UserName = "AtronUser@Tracker";
                applicationUser.Email = "atroUser@email.com";
                applicationUser.NormalizedUserName = "ATRON_USER@TRACKER";
                applicationUser.NormalizedEmail = "ATRON_USER@TRACKER";
                applicationUser.EmailConfirmed = true;
                applicationUser.LockoutEnabled = false;
                applicationUser.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult identityResult = _userManager.CreateAsync(applicationUser, "AtronUser#2024").Result;

                if (identityResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(applicationUser, "AtronUser").Wait();
                }
            }

            // Admin
            if (_userManager.FindByEmailAsync("atronAdmin@email.com").Result == null)
            {
                var applicationUser = new ApplicationUser();
                applicationUser.UserName = "AtronAdmin@Tracker";
                applicationUser.Email = "atronAdmin@email.com";
                applicationUser.NormalizedUserName = "ATRON_ADMIN@TRACKER";
                applicationUser.NormalizedEmail = "ATRON_ADMIN@TRACKER";
                applicationUser.EmailConfirmed = true;
                applicationUser.LockoutEnabled = false;
                applicationUser.SecurityStamp = Guid.NewGuid().ToString();

                IdentityResult identityResult = _userManager.CreateAsync(applicationUser, "AtronAdmin#2024").Result;

                if (identityResult.Succeeded)
                {
                    _userManager.AddToRoleAsync(applicationUser, "AtronAdmin").Wait();
                }
            }
        }

        public void CreateDefaultRoles()
        {
            // Use
            if (!_roleManager.RoleExistsAsync("AtronUser").Result)
            {
                var role = new ApplicationRole()
                {
                    Name = "AtronUser",
                    NormalizedName = "ATRON_USER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                _ = _roleManager.CreateAsync(role).Result;
            }

            // Admin
            if (!_roleManager.RoleExistsAsync("AtronAdmin").Result)
            {
                var role = new ApplicationRole()
                {
                    Name = "AtronAdmin",
                    NormalizedName = "ATRON_ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };

                _ = _roleManager.CreateAsync(role).Result;
            }
        }
    }
}