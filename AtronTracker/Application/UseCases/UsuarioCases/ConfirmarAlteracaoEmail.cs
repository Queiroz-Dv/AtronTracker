using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Application.Interfaces.Services;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class ConfirmarAlteracaoEmail(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        ICacheUsuarioService cacheUsuarioService)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly ICacheUsuarioService _cacheUsuarioService = cacheUsuarioService;

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string emailNovo, string token)
        {
            if (string.IsNullOrWhiteSpace(emailNovo) || string.IsNullOrWhiteSpace(token))
                return Resultado.Falha(UsuarioResource.ErroDadosConfirmacaoAlteracaoEmail);

            var emailJaExiste = await _usuarioRepository.VerificarEmailExistenteAsync(emailNovo);
            if (emailJaExiste)
                return Resultado.Falha(UsuarioResource.ErroEmailEmUso);

            var confirmado = await _usuarioIdentityRepository.ConfirmarAlteracaoEmailAsync(codigoUsuario, emailNovo, token);
            if (!confirmado)
                return Resultado.Falha(UsuarioResource.ErroConfirmacaoAlteracaoEmail);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuario == null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            usuario.Email = emailNovo;
            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuario.Codigo);

            return Resultado.Sucesso(UsuarioResource.MensagemEmailAlterado);
        }
    }
}
