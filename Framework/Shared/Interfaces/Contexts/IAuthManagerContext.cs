using Atron.Domain.Interfaces.ApplicationInterfaces;
using Microsoft.AspNetCore.Identity;
using Shared.Interfaces.Services;
using Shared.Models.ApplicationModels;

namespace Shared.Interfaces.Contexts
{
    /// <summary>
    /// Exemplo de facade para gerenciar autenticação e usuários.
    /// </summary>
    public interface IAuthManagerContext
    {
        SignInManager<ApplicationUser> SignInManager { get; }

        UserManager<ApplicationUser> UserManager { get; }

        ITokenService TokenService { get; }

        IAppUserRepository AppUserRepository { get; }
    }
}