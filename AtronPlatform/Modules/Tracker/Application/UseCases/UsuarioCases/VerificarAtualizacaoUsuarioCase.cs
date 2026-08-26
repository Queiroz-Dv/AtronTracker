using Application.DTO.Request;
using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public sealed class VerificarAtualizacaoUsuarioCase(
        IValidador<UsuarioRequest> validador,
        IUsuarioRepository usuarioRepository)
    {
        private readonly IValidador<UsuarioRequest> _validador = validador;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado<Usuario>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<Usuario>.Falhas(mensagens);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo);
            return usuario is null
                ? Resultado<Usuario>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<Usuario>.Sucesso(usuario);
        }
    }
}
