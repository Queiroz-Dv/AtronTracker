using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.ApiInterfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task RegistrarUsuario(UsuarioRegistroDTO register);
    }
}