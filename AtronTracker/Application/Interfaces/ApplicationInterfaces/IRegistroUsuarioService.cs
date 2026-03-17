using Application.DTO.ApiDTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task<Resultado> RegistrarUsuario(UsuarioRegistroDTO register);
        Task<bool> ConfirmarEmail(string codigoUsuario, string token);
    }
}