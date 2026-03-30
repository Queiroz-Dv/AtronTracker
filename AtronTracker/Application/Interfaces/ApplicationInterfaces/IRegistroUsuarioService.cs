using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest registroRequest);
        Task<Resultado> ConfirmarEmail(string codigoUsuario, string token);
    }
}