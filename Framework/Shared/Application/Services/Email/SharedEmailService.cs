using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Net;
using System.Net.Mail;

namespace Shared.Application.Services.Email
{
    /// <summary>
    /// Implementação compartilhada do serviço de e-mail usando SmtpClient.
    /// Suporta Gmail, Outlook (Hotmail/Live/MSN) e Yahoo.
    /// </summary>
    public class SharedEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IValidador<EmailRequest> _validador;
        private readonly EmailProvider _provider;
        private readonly EmailProviderSettings? _providerSettings;

        public SharedEmailService(IOptions<EmailSettings> settings, IValidador<EmailRequest> validador)
        {
            _settings = settings.Value;

            try
            {
                var providerData = EmailProviderIdentifier.IdentificarEObterConfiguracoes(_settings.FromEmail);
                if (!EmailProviderIdentifier.Messages.HasErrors())
                {
                    (_provider, _providerSettings) = providerData;
                }
            }
            catch
            {
                _provider = EmailProvider.Desconhecido;
                _providerSettings = null;
            }

            _validador = validador;
        }                
        
        public async Task<Resultado> EnviarAsync(EmailRequest message)
        {
            var messages = _validador.Validar(message);
            if (messages.Any()) return Resultado.Falha(messages);

            var mail = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName)
            };

            foreach (var destinatario in message.EmailsDestino)
            {
                if (string.IsNullOrWhiteSpace(destinatario)) continue;
                mail.To.Add(new MailAddress(destinatario));
            }

            mail.Subject = message.Assunto ?? string.Empty;
            mail.Body = message.Mensagem ?? string.Empty;
            mail.IsBodyHtml = true;
           
            var smtpHost = _providerSettings?.SmtpHost ?? _settings.SmtpServer;
            var smtpPort = _providerSettings?.SmtpPort ?? _settings.SmtpPort;
            var enableSsl = _providerSettings?.UseSSL ?? _settings.UseSsl;

            if (string.IsNullOrWhiteSpace(smtpHost))
            {
                return Resultado.Falha("SMTP host is not configured. Check EmailSettings or provider recognition.");
            }

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,                
                Credentials = new NetworkCredential(string.IsNullOrWhiteSpace(_settings.UserName) ? _settings.FromEmail : _settings.UserName, _settings.Password)
            };

            try
            {
                
                await client.SendMailAsync(mail);
                return Resultado.Sucesso();
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao enviar e-mail: {ex.Message}");
            }
        }
    }
}
