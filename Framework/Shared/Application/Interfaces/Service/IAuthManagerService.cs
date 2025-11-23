using Microsoft.AspNetCore.Identity;
using Shared.Domain.Entities.Identity;

namespace Shared.Application.Interfaces.Service
{
    /// <summary>
    /// Exemplo de facade para gerenciar autenticação e usuários.
    /// </summary>
    public interface IAuthManagerService
    {
        SignInManager<ApplicationUser> SignInManager { get; }

        UserManager<ApplicationUser> UserManager { get; }
    }
}