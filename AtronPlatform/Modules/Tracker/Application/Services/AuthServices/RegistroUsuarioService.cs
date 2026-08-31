using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.UseCases.UsuarioCases;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService(
        CadastrarUsuarioCase cadastrarUsuarioCase,
        ConfirmarEmailCase confirmarEmailCase,
        IRecuperacaoSenhaService recuperacaoSenhaService) : IRegistroUsuarioService
    {
        public Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest registroRequest)
            => cadastrarUsuarioCase.ExecutarAsync(registroRequest);

        public Task<Resultado> ConfirmarEmail(string codigoUsuario, string identificador)
            => confirmarEmailCase.ExecutarAsync(codigoUsuario, identificador);

        public Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request)
            => recuperacaoSenhaService.SolicitarAsync(request);

        public Task<Resultado> TrocarSenha(RedefinirSenhaRequest request)
            => recuperacaoSenhaService.TrocarAsync(request);
    }
}
