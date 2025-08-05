using Atron.Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task RegistrarUsuario(UsuarioRegistroDTO register);
    }
}