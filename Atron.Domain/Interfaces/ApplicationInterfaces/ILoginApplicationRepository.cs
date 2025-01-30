using Atron.Domain.ApiEntities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface ILoginApplicationRepository
    {
        Task<bool> AuthenticateUserLoginAsync(ApiLogin login);

        Task Logout();
    }
}