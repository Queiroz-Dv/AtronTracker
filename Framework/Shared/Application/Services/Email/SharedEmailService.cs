using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.Email;
using Shared.Application.Interfaces.Service;
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
        private readonly EmailProvider _provider;
        private readonly EmailProviderSettings? _providerSettings;

        public SharedEmailService(IOptions<EmailSettings> settings)
        {
            _settings = settings.Value;

            try
            {
                (_provider, _providerSettings) = EmailProviderIdentifier.IdentificarEObterConfiguracoes(_settings.FromEmail);
            }
            catch
            {
                _provider = EmailProvider.Desconhecido;
                _providerSettings = null;
            }
        }

        /// <summary>
        /// Obtém o provedor de e-mail identificado.
        /// </summary>
        public EmailProvider ProvedorIdentificado => _provider;

        /// <summary>
        /// Indica se o serviço está configurado corretamente.
        /// </summary>
        public bool EstaConfigurado =>
            !string.IsNullOrWhiteSpace(_settings.FromEmail) &&
            !string.IsNullOrWhiteSpace(_settings.Password) &&
            _provider != EmailProvider.Desconhecido &&
            _providerSettings != null;

        /// <inheritdoc/>
        public async Task EnviarAsync(EmailMessage message)
        {
            ValidarConfiguracao();

            if (message == null) throw new ArgumentNullException(nameof(message));
            if (message.To == null || message.To.Count == 0)
                throw new InvalidOperationException("Nenhum destinatário informado.");

            using var mail = new MailMessage();
            mail.From = new MailAddress(_settings.FromEmail, _settings.FromName);

            foreach (var destinatario in message.To)
            {
                if (string.IsNullOrWhiteSpace(destinatario)) continue;
                mail.To.Add(new MailAddress(destinatario));
            }

            mail.Subject = message.Subject ?? string.Empty;
            mail.Body = message.Body ?? string.Empty;
            mail.IsBodyHtml = true;

            using var client = new SmtpClient(_providerSettings!.SmtpHost, _providerSettings.SmtpPort);
            client.EnableSsl = _providerSettings.UseSSL;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_settings.FromEmail, _settings.Password);

            await client.SendMailAsync(mail);
        }

        /// <summary>
        /// Valida se as configurações do e-mail estão preenchidas.
        /// </summary>
        private void ValidarConfiguracao()
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                throw new InvalidOperationException("O e-mail do remetente não foi configurado.");
            }

            if (string.IsNullOrWhiteSpace(_settings.Password))
            {
                throw new InvalidOperationException("A senha do remetente não foi configurada.");
            }

            if (string.IsNullOrWhiteSpace(_settings.FromName))
            {
                throw new InvalidOperationException("O nome do remetente não foi configurado.");
            }

            if (_providerSettings == null)
            {
                throw new InvalidOperationException("Provedor de e-mail não identificado.");
            }
        }
    }
}
