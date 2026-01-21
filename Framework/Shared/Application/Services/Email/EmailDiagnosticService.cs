using Microsoft.Extensions.Options;
using Shared.Application.DTOS.Email;
using Shared.Application.DTOS.Requests;
using Shared.Application.DTOS.Responses;
using Shared.Application.Email;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;

namespace Shared.Application.Services.Email
{
    /// <summary>
    /// Serviço para diagnóstico de configurações de e-mail.
    /// </summary>
    public class EmailDiagnosticService : IEmailDiagnosticService
    {
        private readonly IEmailService _emailService;
        private readonly EmailSettings _settings;
        private readonly EmailProvider _provider;
        private readonly EmailProviderSettings? _providerSettings;

        public EmailDiagnosticService(
            IEmailService emailService,
            IOptions<EmailSettings> settings)
        {
            _emailService = emailService;
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
        }

        /// <inheritdoc/>
        public async Task<EmailStatusResponse> EnviarDiagnosticoAsync(EmailRequest request)
        {
            if (request.EmailsDestino.Count < 0)
            {
                return EmailStatusResponse.CriarErro(
                    "E-mail de destino é obrigatório.",
                    "O campo 'emailDestino' deve ser preenchido.");
            }

            var configResult = await VerificarConfiguracaoAsync();
            if (!configResult.Sucesso)
            {
                return configResult;
            }

            try
            {
                var assunto = string.IsNullOrWhiteSpace(request.Assunto)
                    ? $"[DIAGNÓSTICO] Verificação do serviço de e-mail - {DateTime.Now:dd/MM/yyyy HH:mm}"
                    : request.Assunto;

                var corpo = GerarCorpoEmailDiagnostico(request.Mensagem);

                await _emailService.EnviarAsync(request);

                return EmailStatusResponse.CriarSucesso(
                    _provider.ToString(),
                    _providerSettings!.SmtpHost,
                    _providerSettings.SmtpPort,
                    _settings.FromEmail,
                    request.EmailsDestino);
            }
            catch (Exception ex)
            {
                return EmailStatusResponse.CriarErro(
                    "Falha ao enviar e-mail de diagnóstico.",
                    $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        public Task<EmailStatusResponse> VerificarConfiguracaoAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "E-mail do remetente não configurado.",
                    "Configure 'EmailSettings:FromEmail' no appsettings.json."));
            }

            if (string.IsNullOrWhiteSpace(_settings.Password))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "Senha do remetente não configurada.",
                    "Configure 'EmailSettings:Password' no appsettings.json."));
            }

            if (string.IsNullOrWhiteSpace(_settings.FromName))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "Nome do remetente não configurado.",
                    "Configure 'EmailSettings:FromName' no appsettings.json."));
            }

            if (_provider == EmailProvider.Desconhecido || _providerSettings == null)
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "Provedor de e-mail não suportado.",
                    $"O domínio do e-mail '{_settings.FromEmail}' não é suportado. " +
                    "Provedores suportados: Gmail, Outlook (Hotmail/Live/MSN), Yahoo."));
            }

            return Task.FromResult(EmailStatusResponse.CriarStatus(
                true,
                _provider.ToString(),
                _providerSettings.SmtpHost,
                _providerSettings.SmtpPort,
                _settings.FromEmail));
        }

        /// <inheritdoc/>
        public Task<EmailStatusResponse> ObterStatusAsync()
        {
            var operacional =
                !string.IsNullOrWhiteSpace(_settings.FromEmail) &&
                !string.IsNullOrWhiteSpace(_settings.Password) &&
                _provider != EmailProvider.Desconhecido &&
                _providerSettings != null;

            if (!operacional)
            {
                return Task.FromResult(new EmailStatusResponse
                {
                    Sucesso = false,
                    ServicoOperacional = false,
                    Mensagem = "Serviço de e-mail não está configurado corretamente.",
                    DataOperacao = DateTime.Now
                });
            }

            return Task.FromResult(EmailStatusResponse.CriarStatus(
                true,
                _provider.ToString(),
                _providerSettings!.SmtpHost,
                _providerSettings.SmtpPort,
                _settings.FromEmail));
        }

        /// <summary>
        /// Gera o corpo HTML do e-mail de diagnóstico.
        /// </summary>
        private string GerarCorpoEmailDiagnostico(string? mensagemPersonalizada)
        {
            var mensagem = string.IsNullOrWhiteSpace(mensagemPersonalizada)
                ? "Este é um e-mail de diagnóstico para validar a configuração de envio do Sistema Atron."
                : mensagemPersonalizada;

            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #17a2b8; }}
        .header h1 {{ color: #17a2b8; margin: 0; }}
        .content {{ padding: 20px 0; }}
        .content p {{ color: #333; line-height: 1.6; }}
        .info-box {{ background-color: #e7f5f8; padding: 15px; border-radius: 5px; margin: 15px 0; }}
        .info-box p {{ margin: 5px 0; font-size: 14px; }}
        .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>📧 Diagnóstico de E-mail</h1>
        </div>
        <div class='content'>
            <p>{mensagem}</p>
            <div class='info-box'>
                <p><strong>Provedor:</strong> {_provider}</p>
                <p><strong>SMTP Host:</strong> {_providerSettings?.SmtpHost ?? "N/A"}</p>
                <p><strong>SMTP Port:</strong> {_providerSettings?.SmtpPort.ToString() ?? "N/A"}</p>
                <p><strong>Remetente:</strong> {_settings.FromEmail}</p>
                <p><strong>Data/Hora:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
            </div>
            <p>Se você recebeu este e-mail, o serviço de e-mail está funcionando corretamente!</p>
        </div>
        <div class='footer'>
            <p>Módulo AtronEmail - Diagnóstico Interno</p>
            <p>&copy; {DateTime.Now.Year} Sistema Atron</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
