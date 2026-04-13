using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class AlterarEmail
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;

        public AlterarEmail(
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
        }

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string emailNovo, string urlBase)
        {
            if (string.IsNullOrWhiteSpace(emailNovo))
                return Resultado.Falha("O novo e-mail não pode ser vazio.");

            var emailJaExiste = await _usuarioRepository.VerificarEmailExistenteAsync(emailNovo);
            if (emailJaExiste)
                return Resultado.Falha("Este e-mail já está em uso por outro usuário.");

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuario == null)
                return Resultado.Falha("Usuário não encontrado.");

            var token = await _usuarioIdentityRepository.GerarTokenAlteracaoEmailAsync(codigoUsuario, emailNovo);

            string link = $"{urlBase}/confirmar-alteracao-email?usuarioCodigo={codigoUsuario}&emailNovo={emailNovo}&token={token}";

            try
            {
                await _emailService.EnviarAsync(new EmailRequest
                {
                    Assunto = "Confirme a alteração do seu e-mail - AtronTracker",
                    Mensagem = CorpoDoEmail(usuario.Nome, link),
                    EmailsDestino = [emailNovo]
                });
            }
            catch { }

            return Resultado.Sucesso("Solicitação de alteração enviada. Verifique o novo e-mail para confirmar.");
        }

        private static string CorpoDoEmail(string nomeUsuario, string link)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h1 style='color: #2c3e50;'>Alteração de E-mail</h1>
                    <p>Olá, <strong>{nomeUsuario}</strong>!</p>
                    <p>Recebemos uma solicitação para alterar o e-mail da sua conta. Clique no botão abaixo para confirmar:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirmar novo E-mail</a>
                    </div>
                    <p style='font-size: 12px; color: #999; word-break: break-all;'>{link}</p>
                    <p style='font-size: 12px; color: #aaa;'>Se você não solicitou esta alteração, ignore este e-mail.</p>
                </div>";
        }
    }
}