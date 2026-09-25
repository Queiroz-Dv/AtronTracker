using Application.DTO.Request;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class VerificarUsuarioCase(
        IValidador<UsuarioRequest> validador,
        VerificarUsuarioExistenteCase verificarUsuarioExistenteCase)
    {
        private readonly IValidador<UsuarioRequest> _validador = validador;
        private readonly VerificarUsuarioExistenteCase _verificarUsuarioExistenteCase = verificarUsuarioExistenteCase;

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            var resultadoVerificacao = await _verificarUsuarioExistenteCase.ExecutarAsync(request.Codigo, request.Email);

            if (resultadoVerificacao.TeveFalha)
                return Resultado<UsuarioRequest>.Falha(request, resultadoVerificacao.Messages);

            return Resultado<UsuarioRequest>.Sucesso(request);
        }
    }
}