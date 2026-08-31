using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.UseCases.UsuarioCases;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService(
        CadastrarUsuarioCase cadastrarUsuarioCase,
        ConfirmarEmailCase confirmarEmailCase,
        SolicitarRecuperacaoSenhaCase solicitarRecuperacaoSenhaCase,
        TrocarSenhaCase trocarSenhaCase) : IRegistroUsuarioService
    {
        public Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest registroRequest)
            => cadastrarUsuarioCase.ExecutarAsync(registroRequest);

        public Task<Resultado> ConfirmarEmail(string codigoUsuario, string identificador)
            => confirmarEmailCase.ExecutarAsync(codigoUsuario, identificador);

        public Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request)
            => solicitarRecuperacaoSenhaCase.ExecutarAsync(request);

        public Task<Resultado> TrocarSenha(RedefinirSenhaRequest request)
            => trocarSenhaCase.ExecutarAsync(request);
    }
}
