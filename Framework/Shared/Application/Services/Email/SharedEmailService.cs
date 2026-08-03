using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Net;
using System.Net.Http.Json;
using System.Net.Mail;
using System.Text.Json.Serialization;

namespace Shared.Application.Services.Email
{
    /// <summary>
    /// Implementacao compartilhada do servico de e-mail.
    /// </summary>
    public class SharedEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly IValidador<EmailRequest> _validador;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly EmailProvider _provider;
        private readonly EmailProviderSettings? _providerSettings;

        public SharedEmailService(
            IOptions<EmailSettings> settings,
            IValidador<EmailRequest> validador,
            IHttpClientFactory httpClientFactory)
        {
            _settings = settings.Value;
            _httpClientFactory = httpClientFactory;

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

            return UsarBrevo()
                ? await EnviarBrevoAsync(message)
                : await EnviarSmtpAsync(message);
        }

        private async Task<Resultado> EnviarSmtpAsync(EmailRequest message)
        {
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
                return Resultado.Falha("SMTP host is not configured. Check EmailSettings or provider recognition.");

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    string.IsNullOrWhiteSpace(_settings.UserName) ? _settings.FromEmail : _settings.UserName,
                    _settings.Password)
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

        private async Task<Resultado> EnviarBrevoAsync(EmailRequest message)
        {
            if (string.IsNullOrWhiteSpace(_settings.Brevo.ApiKey))
                return Resultado.Falha("Brevo API key nao configurada. Configure EmailSettings:Brevo:ApiKey.");

            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
                return Resultado.Falha("E-mail do remetente nao configurado. Configure EmailSettings:FromEmail.");

            var baseUrl = string.IsNullOrWhiteSpace(_settings.Brevo.BaseUrl)
                ? EmailTransportCatalog.BrevoBaseUrl
                : _settings.Brevo.BaseUrl.TrimEnd('/');

            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/smtp/email");
            request.Headers.Add("api-key", _settings.Brevo.ApiKey);
            request.Headers.Add("accept", "application/json");
            request.Content = JsonContent.Create(new BrevoEmailRequest
            {
                Sender = new BrevoEmailSender
                {
                    Name = _settings.FromName,
                    Email = _settings.FromEmail
                },
                To = message.EmailsDestino
                    .Where(destinatario => !string.IsNullOrWhiteSpace(destinatario))
                    .Select(destinatario => new BrevoEmailRecipient { Email = destinatario })
                    .ToList(),
                Subject = message.Assunto ?? string.Empty,
                HtmlContent = message.Mensagem ?? string.Empty
            });

            try
            {
                using var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                    return Resultado.Sucesso();

                var detalhe = await response.Content.ReadAsStringAsync();
                return Resultado.Falha($"Erro ao enviar e-mail pela Brevo: {(int)response.StatusCode} {response.ReasonPhrase}. {detalhe}");
            }
            catch (Exception ex)
            {
                return Resultado.Falha($"Erro ao enviar e-mail pela Brevo: {ex.Message}");
            }
        }

        private bool UsarBrevo()
            => string.Equals(_settings.Provider, EmailTransportCatalog.BrevoProvider, StringComparison.OrdinalIgnoreCase);

        private sealed class BrevoEmailRequest
        {
            [JsonPropertyName("sender")]
            public BrevoEmailSender Sender { get; set; }

            [JsonPropertyName("to")]
            public List<BrevoEmailRecipient> To { get; set; } = [];

            [JsonPropertyName("subject")]
            public string Subject { get; set; }

            [JsonPropertyName("htmlContent")]
            public string HtmlContent { get; set; }
        }

        private sealed class BrevoEmailSender
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }

            [JsonPropertyName("email")]
            public string Email { get; set; }
        }

        private sealed class BrevoEmailRecipient
        {
            [JsonPropertyName("email")]
            public string Email { get; set; }
        }
    }
}
