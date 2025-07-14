using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    public interface IRegisterUserService
    {
       // Task<bool> EmailExists(string email);
        Task<UsuarioRegistroDTO> RegisterUser(UsuarioRegistroDTO register);
       // Task<bool> UserExists(string code);
    }
}