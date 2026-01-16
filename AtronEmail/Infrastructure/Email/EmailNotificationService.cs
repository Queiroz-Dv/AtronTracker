using AtronEmail.Application.Interfaces;
using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.Interfaces.Service;

namespace AtronEmail.Infrastructure.Email
{
    /// <summary>
    /// Serviço de notificação por e-mail.
    /// Integra com o padrão de notificações para enviar e-mails a usuários superiores.
    /// </summary>
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailService _emailService;
        private readonly EmailSettings _settings;

        public EmailNotificationService(
            IEmailService emailService,
            IOptions<EmailSettings> settings)
        {
            _emailService = emailService;
            _settings = settings.Value;
        }

        /// <inheritdoc/>
        public async Task EnviarAsync(EmailMessage message)
        {
            await _emailService.EnviarAsync(message);
        }

        /// <inheritdoc/>
        public async Task EnviarNotificacaoAsync(string assunto, string mensagem, IEnumerable<string> destinatarios)
        {
            if (destinatarios == null || !destinatarios.Any())
            {
                return;
            }

            var message = new EmailMessage
            {
                To = destinatarios.ToList(),
                Subject = assunto,
                Body = GerarCorpoNotificacao(mensagem)
            };

            await _emailService.EnviarAsync(message);
        }

        /// <inheritdoc/>
        public async Task NotificarSuperioresAsync(string tipoEvento, string descricao, string entidade, IEnumerable<string> emailsSuperiores)
        {
            if (emailsSuperiores == null || !emailsSuperiores.Any())
            {
                return;
            }

            var assunto = $"[NOTIFICAÇÃO] {tipoEvento} - {entidade}";
            var corpo = GerarCorpoNotificacaoSuperior(tipoEvento, descricao, entidade);

            var message = new EmailMessage
            {
                To = emailsSuperiores.ToList(),
                Subject = assunto,
                Body = corpo
            };

            await _emailService.EnviarAsync(message);
        }

        /// <summary>
        /// Gera o corpo HTML de uma notificação genérica.
        /// </summary>
        private string GerarCorpoNotificacao(string mensagem)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #007bff; }}
        .header h1 {{ color: #007bff; margin: 0; }}
        .content {{ padding: 20px 0; }}
        .content p {{ color: #333; line-height: 1.6; }}
        .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🔔 Notificação do Sistema</h1>
        </div>
        <div class='content'>
            <p>{mensagem}</p>
            <p><small>Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</small></p>
        </div>
        <div class='footer'>
            <p>Este é um e-mail automático do Sistema Atron.</p>
            <p>&copy; {DateTime.Now.Year} Sistema Atron</p>
        </div>
    </div>
</body>
</html>";
        }

        /// <summary>
        /// Gera o corpo HTML de uma notificação para superiores sobre evento crítico.
        /// </summary>
        private string GerarCorpoNotificacaoSuperior(string tipoEvento, string descricao, string entidade)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #dc3545; }}
        .header h1 {{ color: #dc3545; margin: 0; }}
        .content {{ padding: 20px 0; }}
        .content p {{ color: #333; line-height: 1.6; }}
        .event-box {{ background-color: #fff3cd; padding: 15px; border-radius: 5px; margin: 15px 0; border-left: 4px solid #ffc107; }}
        .event-box p {{ margin: 5px 0; font-size: 14px; }}
        .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>⚠️ Notificação para Gestão</h1>
        </div>
        <div class='content'>
            <p>Uma ação importante foi realizada no sistema e requer sua atenção:</p>
            <div class='event-box'>
                <p><strong>Tipo de Evento:</strong> {tipoEvento}</p>
                <p><strong>Entidade Afetada:</strong> {entidade}</p>
                <p><strong>Descrição:</strong> {descricao}</p>
                <p><strong>Data/Hora:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
            </div>
            <p>Por favor, revise esta ação conforme necessário.</p>
        </div>
        <div class='footer'>
            <p>Este é um e-mail automático do Sistema Atron - Módulo de Notificações.</p>
            <p>&copy; {DateTime.Now.Year} Sistema Atron</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
