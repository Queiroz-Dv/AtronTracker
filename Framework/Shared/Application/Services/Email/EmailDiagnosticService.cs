using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.DTOS.Responses;
using Shared.Application.Email;
using Shared.Application.Email.Models;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Shared.Application.Services.Email
{
    /// <summary>
    /// Servico para diagnostico de configuracoes de e-mail.
    /// </summary>
    public class EmailDiagnosticService : IEmailDiagnosticService
    {
        private readonly IEmailService _emailService;
        private readonly EmailSettings _settings;
        private readonly EmailProvider _provider;
        private readonly EmailProviderSettings? _providerSettings;
        private readonly IEmailTemplateRenderer _templateRenderer;

        public EmailDiagnosticService(
            IEmailService emailService,
            IOptions<EmailSettings> settings,
            IEmailTemplateRenderer templateRenderer)
        {
            _emailService = emailService;
            _settings = settings.Value;
            _templateRenderer = templateRenderer;

            try
            {
                var providerData = EmailProviderIdentifier.IdentificarEObterConfiguracoes(_settings.FromEmail);
                if (!EmailProviderIdentifier.Messages.TemErros())
                {
                    (_provider, _providerSettings) = providerData;
                }
            }
            catch
            {
                _provider = EmailProvider.Desconhecido;
                _providerSettings = null;
            }
        }

        public async Task<EmailStatusResponse> EnviarDiagnosticoAsync(EmailRequest request)
        {
            if (request.EmailsDestino is null || request.EmailsDestino.Count == 0)
            {
                return EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_DestinoObrigatorio,
                    EmailDiagnosticResource.Detalhe_DestinoObrigatorio);
            }

            var configResult = await VerificarConfiguracaoAsync();
            if (!configResult.Sucesso)
                return configResult;

            try
            {
                request.Assunto = string.IsNullOrWhiteSpace(request.Assunto)
                    ? string.Format(EmailDiagnosticResource.Assunto_DiagnosticoPadrao, DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                    : request.Assunto;

                var emailComposto = ComporEmailDiagnostico(request);
                if (emailComposto.TeveFalha)
                {
                    return EmailStatusResponse.CriarErro(
                        EmailDiagnosticResource.Erro_EnvioDiagnostico,
                        string.Join(" | ", emailComposto.Messages.Select(mensagem => mensagem.Descricao)));
                }

                request = emailComposto.Dados;

                var envio = await _emailService.EnviarAsync(request);
                if (envio.TeveFalha)
                {
                    return EmailStatusResponse.CriarErro(
                        EmailDiagnosticResource.Erro_EnvioDiagnostico,
                        string.Join(" | ", envio.Messages.Select(mensagem => mensagem.Descricao)));
                }

                return EmailStatusResponse.CriarSucesso(
                    ObterProviderConfigurado(),
                    ObterHostStatus(),
                    ObterPortaStatus(),
                    _settings.FromEmail,
                    request.EmailsDestino);
            }
            catch (Exception ex)
            {
                return EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_EnvioDiagnostico,
                    $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        public Task<EmailStatusResponse> VerificarConfiguracaoAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_RemetenteNaoConfigurado,
                    "Configure 'EmailSettings:FromEmail'."));
            }

            if (string.IsNullOrWhiteSpace(_settings.FromName))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_NomeRemetenteNaoConfigurado,
                    "Configure 'EmailSettings:FromName'."));
            }

            if (UsarBrevo())
            {
                if (string.IsNullOrWhiteSpace(_settings.Brevo.ApiKey))
                {
                    return Task.FromResult(EmailStatusResponse.CriarErro(
                        EmailDiagnosticResource.Erro_ApiKeyBrevoNaoConfigurada,
                        "Configure 'EmailSettings:Brevo:ApiKey' via variavel de ambiente ou secrets."));
                }

                return Task.FromResult(EmailStatusResponse.CriarStatus(
                    true,
                    "Brevo",
                    ObterHostStatus(),
                    443,
                    _settings.FromEmail));
            }

            if (string.IsNullOrWhiteSpace(_settings.Password))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_SenhaRemetenteNaoConfigurada,
                    "Configure 'EmailSettings:Password'."));
            }

            if (_provider == EmailProvider.Desconhecido || _providerSettings == null)
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    EmailDiagnosticResource.Erro_ProvedorNaoSuportado,
                    $"O dominio do e-mail '{_settings.FromEmail}' nao e suportado. " +
                    "Provedores suportados: Gmail, Outlook (Hotmail/Live/MSN), Yahoo."));
            }

            return Task.FromResult(EmailStatusResponse.CriarStatus(
                true,
                _provider.ToString(),
                _providerSettings.SmtpHost,
                _providerSettings.SmtpPort,
                _settings.FromEmail));
        }

        public Task<EmailStatusResponse> ObterStatusAsync()
        {
            var operacional =
                !string.IsNullOrWhiteSpace(_settings.FromEmail) &&
                !string.IsNullOrWhiteSpace(_settings.FromName) &&
                (UsarBrevo()
                    ? !string.IsNullOrWhiteSpace(_settings.Brevo.ApiKey)
                    : !string.IsNullOrWhiteSpace(_settings.Password) &&
                      _provider != EmailProvider.Desconhecido &&
                      _providerSettings != null);

            if (!operacional)
            {
                return Task.FromResult(new EmailStatusResponse
                {
                    Sucesso = false,
                    ServicoOperacional = false,
                    Mensagem = EmailDiagnosticResource.Mensagem_ServicoNaoConfigurado,
                    DataOperacao = DateTime.Now
                });
            }

            return Task.FromResult(EmailStatusResponse.CriarStatus(
                true,
                ObterProviderConfigurado(),
                ObterHostStatus(),
                ObterPortaStatus(),
                _settings.FromEmail));
        }

        private bool UsarBrevo()
            => string.Equals(_settings.Provider, EmailTransportCatalog.BrevoProvider, StringComparison.OrdinalIgnoreCase);

        private string ObterProviderConfigurado()
            => UsarBrevo() ? EmailTransportCatalog.BrevoProvider : _provider.ToString();

        private string ObterHostStatus()
            => UsarBrevo()
                ? (string.IsNullOrWhiteSpace(_settings.Brevo.BaseUrl) ? EmailTransportCatalog.BrevoBaseUrl : _settings.Brevo.BaseUrl)
                : _providerSettings?.SmtpHost ?? _settings.SmtpServer ?? string.Empty;

        private int ObterPortaStatus()
            => UsarBrevo() ? 443 : _providerSettings?.SmtpPort ?? _settings.SmtpPort;

        private Resultado<EmailRequest> ComporEmailDiagnostico(EmailRequest request)
        {
            var mensagem = string.IsNullOrWhiteSpace(request.Mensagem)
                ? EmailDiagnosticResource.Mensagem_CorpoPadrao
                : request.Mensagem;

            var assunto = string.IsNullOrWhiteSpace(request.Assunto)
                ? string.Format(EmailDiagnosticResource.Assunto_DiagnosticoPadrao, DateTime.Now.ToString("dd/MM/yyyy HH:mm"))
                : request.Assunto;

            return _templateRenderer.Renderizar(
                new EmailTemplateDefinition(
                    typeof(EmailDiagnosticService).Assembly,
                    EmailTemplateResourceNames.Diagnostico,
                    assunto,
                    "Diagnóstico de e-mail"),
                new EmailDiagnosticoModel
                {
                    Mensagem = mensagem,
                    Provedor = ObterProviderConfigurado(),
                    Host = ObterHostStatus(),
                    Remetente = _settings.FromEmail,
                    DataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")
                },
                request.EmailsDestino);
        }
    }
}
