using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Interfaces.Contexts;
using Shared.Models.ApplicationModels;

namespace Shared.Services.Facades
{
    /// <summary>
    /// Facade concreto para gerenciar autenticação e usuários.
    /// </summary>
    public class AuthManagerContext : IAuthManagerContext
    {
        public AuthManagerContext(
            IAppUserRepository appUserRepository,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            AppUserRepository = appUserRepository;
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }

        public IAppUserRepository AppUserRepository { get; }
    }
}