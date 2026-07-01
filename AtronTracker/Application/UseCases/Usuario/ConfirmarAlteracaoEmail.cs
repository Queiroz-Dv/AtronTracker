using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Application.Interfaces.Services;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class ConfirmarAlteracaoEmail
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly ICacheUsuarioService _cacheUsuarioService;

        public ConfirmarAlteracaoEmail(
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            ICacheUsuarioService cacheUsuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _cacheUsuarioService = cacheUsuarioService;
        }

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string emailNovo, string token)
        {
            if (string.IsNullOrWhiteSpace(emailNovo) || string.IsNullOrWhiteSpace(token))
                return Resultado.Falha("Dados inválidos para confirmação.");

            var emailJaExiste = await _usuarioRepository.VerificarEmailExistenteAsync(emailNovo);
            if (emailJaExiste)
                return Resultado.Falha("Este e-mail já está em uso por outro usuário.");

            var confirmado = await _usuarioIdentityRepository.ConfirmarAlteracaoEmailAsync(codigoUsuario, emailNovo, token);
            if (!confirmado)
                return Resultado.Falha("Falha ao confirmar alteração. Token inválido ou expirado.");

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuario == null)
                return Resultado.Falha("Usuário não encontrado.");

            usuario.Email = emailNovo;
            await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuario.Codigo);

            return Resultado.Sucesso("E-mail alterado com sucesso.");
        }
    }
}
