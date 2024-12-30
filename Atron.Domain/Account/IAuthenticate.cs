using System.Threading.Tasks;

namespace Atron.Domain.Account
{
    /// <summary>
    /// Classe de autenticação para os usuários
    /// </summary>
    public interface IAuthenticate
    {
        Task<bool> Authenticate(string email, string password);
        Task<bool> RegisterUser(string email, string password);
        Task Logout();
    }
}