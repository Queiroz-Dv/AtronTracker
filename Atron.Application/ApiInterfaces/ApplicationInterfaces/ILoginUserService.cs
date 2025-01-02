using Atron.Application.DTO.Account;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    /// <summary>
    /// Classe de autenticação para os usuários
    /// </summary>
    public interface ILoginUserService
    {
        Task<LoginDTO> Authenticate(LoginDTO login);

        Task Logout();
    }
}