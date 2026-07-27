using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService(
        ICadastroUsuarioService cadastroUsuarioService,
        IRecuperacaoSenhaService recuperacaoSenhaService) : IRegistroUsuarioService
    {
        public Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest registroRequest)
            => cadastroUsuarioService.RegistrarAsync(registroRequest);

        public Task<Resultado> ConfirmarEmail(string codigoUsuario, string identificador)
            => cadastroUsuarioService.ConfirmarEmailAsync(codigoUsuario, identificador);

        public Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request)
            => recuperacaoSenhaService.SolicitarAsync(request);

        public Task<Resultado> TrocarSenha(RedefinirSenhaRequest request)
            => recuperacaoSenhaService.TrocarAsync(request);
    }
}