using Atron.Domain.ApiEntities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface IRegisterApplicationRepository
    {
        Task<bool> UpdateUserAccountAsync(ApiRegister register);
        Task<bool> RegisterUserAccountAsync(ApiRegister register);
        Task DeleteAccountUserAsync(string userName);
    }
}