using Domain.Interfaces.UsuarioInterfaces;
using Application.Email.Compositores;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class SolicitarReativacao
    {
        private readonly IUsuarioRepository _usuarioRepository;        
        private readonly IEmailService _emailService;
        private readonly IAcessoEmailCompositor _emailCompositor;

        public SolicitarReativacao(
            IUsuarioRepository usuarioRepository,
            IEmailService emailService,
            IAcessoEmailCompositor emailCompositor)
        {
            _usuarioRepository = usuarioRepository;
            _emailService = emailService;
            _emailCompositor = emailCompositor;
        }

        public async Task<Resultado> ExecutarAsync(string email)
        {
            if (email.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var usuario = await _usuarioRepository.ObterInativoPorEmailAsync(email);
            if (usuario is null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            try
            {
                var emailReativacao = _emailCompositor.ComporReativacaoConta(
                    usuario.Email,
                    usuario.Nome,
                    usuario.CodigoReativacao);
                var envio = await _emailService.EnviarAsync(emailReativacao);
                if (envio.TeveFalha)
                    return Resultado.Falha(envio.Messages);
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }

            return Resultado.Sucesso().AdicionarMensagem(UsuarioResource.MensagemCodigoReativacaoEnviado);
        }
    }
}
