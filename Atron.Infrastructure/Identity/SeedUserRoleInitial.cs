using Atron.Domain.Account;
using Microsoft.AspNetCore.Identity;
using System;

namespace Atron.Infrastructure.Identity
{
    public class SeedUserRoleInitial : ISeedUserRoleInitial
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedUsers()
        {
            // Use
            if (_userManager.FindByEmailAsync("atronUser@email.com").Result == null)
            {
                var applicationUser = new ApplicationUser();
                applicationUser.UserName = "AtronUser@Tracker";
                applicationUser.Email = "atroUsern@email.com";
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

        public void SeedRoles()
        {
            // Use
            if (!_roleManager.RoleExistsAsync("AtronUser").Result)
            {
                IdentityRole role = new()
                {
                    Name = "AtronUser",
                    NormalizedName = "ATRON_USER"
                };

                _ = _roleManager.CreateAsync(role).Result;
            }

            // Admin
            if (!_roleManager.RoleExistsAsync("AtronAdmin").Result)
            {
                IdentityRole role = new()
                {
                    Name = "AtronAdmin",
                    NormalizedName = "ATRON_ADMIN"
                };

                _ = _roleManager.CreateAsync(role).Result;
            }
        }

    }
}
