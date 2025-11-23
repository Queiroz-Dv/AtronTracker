using Domain.ApiEntities;
using System.Threading.Tasks;

namespace Domain.Interfaces.ApplicationInterfaces
{
    public interface IRegisterApplicationRepository
    {
        Task<bool> UpdateUserAccountAsync(ApiRegister register);
        Task<bool> RegisterUserAccountAsync(ApiRegister register);
        Task<bool> UserExists(ApiRegister register);
        Task DeleteAccountUserAsync(string userName);
        Task<bool> UserExistsByUserCode(string code);
        Task<bool> UserExistsByEmail(string email);
    }
}