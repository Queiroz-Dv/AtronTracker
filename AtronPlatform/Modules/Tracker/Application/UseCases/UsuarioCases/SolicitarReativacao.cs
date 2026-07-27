using Domain.Interfaces.UsuarioInterfaces;
using Application.Email.Compositores;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class SolicitarReativacao(
        IUsuarioRepository usuarioRepository,
        IEmailService emailService,
        IAcessoEmailCompositor emailCompositor)
    {
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;        
        private readonly IEmailService _emailService = emailService;
        private readonly IAcessoEmailCompositor _emailCompositor = emailCompositor;

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

                if (emailReativacao.TeveFalha)
                    return Resultado.Falha(emailReativacao.Messages);

                var envio = await _emailService.EnviarAsync(emailReativacao.Dados);
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
