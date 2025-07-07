using Microsoft.AspNetCore.Identity;
using Shared.Interfaces.Contexts;
using Shared.Models.ApplicationModels;

namespace Shared.Services.Contexts
{
    /// <summary>
    /// Facade concreto para gerenciar autenticação e usuários.
    /// </summary>
    public class AuthManagerContext : IAuthManagerContext
    {
        public AuthManagerContext(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            SignInManager = signInManager;
            UserManager = userManager;
        }

        public SignInManager<ApplicationUser> SignInManager { get; set; }

        public UserManager<ApplicationUser> UserManager { get; set; }
    }
}