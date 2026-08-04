using Domain.Interfaces.Identity;
using Application.Email.Compositores;
using Application.Interfaces.Services;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class AlterarEmail(
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IEmailService emailService,
        IAcessoEmailCompositor emailCompositor,
        IEnderecoFrontendService enderecoFrontendService)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IAcessoEmailCompositor _emailCompositor = emailCompositor;
        private readonly IEnderecoFrontendService _enderecoFrontendService = enderecoFrontendService;

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string emailNovo)
        {
            if (string.IsNullOrWhiteSpace(emailNovo))
                return Resultado.Falha(UsuarioResource.ErroNovoEmailVazio);

            var emailJaExiste = await _usuarioRepository.VerificarEmailExistenteAsync(emailNovo);
            if (emailJaExiste)
                return Resultado.Falha(UsuarioResource.ErroEmailEmUso);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuario == null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            var token = await _usuarioIdentityRepository.GerarTokenAlteracaoEmailAsync(codigoUsuario, emailNovo);

            var urlBase = _enderecoFrontendService.ObterUriBase();
            string link = $"{urlBase}/confirmar-alteracao-email?usuarioCodigo={codigoUsuario}&emailNovo={emailNovo}&token={token}";

            try
            {
                var email = _emailCompositor.ComporAlteracaoEmail(emailNovo, usuario.Nome, link);
                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                var envio = await _emailService.EnviarAsync(email.Dados);
                if (envio.TeveFalha)
                    return Resultado.Falha(envio.Messages);
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }

            return Resultado.Sucesso(UsuarioResource.MensagemSolicitacaoAlteracaoEmail);
        }
    }
}
