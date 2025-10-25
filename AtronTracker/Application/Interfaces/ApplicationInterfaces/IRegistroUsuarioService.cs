using Application.DTO.ApiDTO;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task RegistrarUsuario(UsuarioRegistroDTO register);
    }
}