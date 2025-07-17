using Atron.Domain.ApiEntities;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces.ApplicationInterfaces
{
    public interface IRegisterApplicationRepository : IRepository<UsuarioRegistro>
    {
        Task<bool> UpdateUserAccountAsync(UsuarioRegistro register);
        Task<bool> RegisterUserAccountAsync(UsuarioRegistro register);
        Task<bool> UserExists(UsuarioRegistro register);
        Task DeleteAccountUserAsync(string userName);
        Task<bool> UserExistsByUserCode(string code);
        Task<bool> UserExistsByEmail(string email);
    }
}