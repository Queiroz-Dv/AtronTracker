using Application.DTO.Request;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface ICadastroUsuarioService
    {
        Task<Resultado> RegistrarAsync(UsuarioRegistroRequest request);

        Task<Resultado> ConfirmarEmailAsync(string codigoUsuario, string identificador);
    }
}
