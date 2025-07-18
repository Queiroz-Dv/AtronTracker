using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
       // Task<bool> EmailExists(string email);
        Task<UsuarioRegistroDTO> RegistrarUsuario(UsuarioRegistroDTO register);
       // Task<bool> UserExists(string code);
    }
}