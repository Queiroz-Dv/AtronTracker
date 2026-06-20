using Application.DTO.Request;
using Shared.Application.DTOS.Auth;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.ApplicationInterfaces
{
    public interface IRegistroUsuarioService
    {
        Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest registroRequest);
        Task<Resultado> ConfirmarEmail(string codigoUsuario, string token);

        Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request);
        Task<Resultado> TrocarSenha(RedefinirSenhaRequest request);
    }
}