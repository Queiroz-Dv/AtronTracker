using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface ILoginApplicationRepository
    {
        Task<bool> AuthenticateUserLoginAsync(ApiLogin login);
        Task<bool> ConfigPasswordAsync(string codigoDoUsuario, ApiLogin apiLogin);

        Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity);

        Task<bool> ObterRefreshTokenDoUsuario(string refreshToken);

        Task Logout();
    }
}