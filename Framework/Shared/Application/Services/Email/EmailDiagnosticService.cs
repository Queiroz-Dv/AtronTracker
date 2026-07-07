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
    /// Servico para diagnostico de configuracoes de e-mail.
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

        public async Task<EmailStatusResponse> EnviarDiagnosticoAsync(EmailRequest request)
        {
            if (request.EmailsDestino is null || request.EmailsDestino.Count == 0)
            {
                return EmailStatusResponse.CriarErro(
                    "E-mail de destino e obrigatorio.",
                    "O campo 'emailsDestino' deve ser preenchido.");
            }

            var configResult = await VerificarConfiguracaoAsync();
            if (!configResult.Sucesso)
                return configResult;

            try
            {
                request.Assunto = string.IsNullOrWhiteSpace(request.Assunto)
                    ? $"[DIAGNOSTICO] Verificacao do servico de e-mail - {DateTime.Now:dd/MM/yyyy HH:mm}"
                    : request.Assunto;

                request.Mensagem = GerarCorpoEmailDiagnostico(request.Mensagem);

                var envio = await _emailService.EnviarAsync(request);
                if (envio.TeveFalha)
                {
                    return EmailStatusResponse.CriarErro(
                        "Falha ao enviar e-mail de diagnostico.",
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
                    "Falha ao enviar e-mail de diagnostico.",
                    $"{ex.GetType().Name}: {ex.Message}");
            }
        }

        public Task<EmailStatusResponse> VerificarConfiguracaoAsync()
        {
            if (string.IsNullOrWhiteSpace(_settings.FromEmail))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "E-mail do remetente nao configurado.",
                    "Configure 'EmailSettings:FromEmail'."));
            }

            if (string.IsNullOrWhiteSpace(_settings.FromName))
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "Nome do remetente nao configurado.",
                    "Configure 'EmailSettings:FromName'."));
            }

            if (UsarBrevo())
            {
                if (string.IsNullOrWhiteSpace(_settings.Brevo.ApiKey))
                {
                    return Task.FromResult(EmailStatusResponse.CriarErro(
                        "API key da Brevo nao configurada.",
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
                    "Senha do remetente nao configurada.",
                    "Configure 'EmailSettings:Password'."));
            }

            if (_provider == EmailProvider.Desconhecido || _providerSettings == null)
            {
                return Task.FromResult(EmailStatusResponse.CriarErro(
                    "Provedor de e-mail nao suportado.",
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
                    Mensagem = "Servico de e-mail nao esta configurado corretamente.",
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
            => string.Equals(_settings.Provider, "Brevo", StringComparison.OrdinalIgnoreCase);

        private string ObterProviderConfigurado()
            => UsarBrevo() ? "Brevo" : _provider.ToString();

        private string ObterHostStatus()
            => UsarBrevo()
                ? (string.IsNullOrWhiteSpace(_settings.Brevo.BaseUrl) ? "https://api.brevo.com/v3" : _settings.Brevo.BaseUrl)
                : _providerSettings?.SmtpHost ?? _settings.SmtpServer ?? string.Empty;

        private int ObterPortaStatus()
            => UsarBrevo() ? 443 : _providerSettings?.SmtpPort ?? _settings.SmtpPort;

        private string GerarCorpoEmailDiagnostico(string? mensagemPersonalizada)
        {
            var mensagem = string.IsNullOrWhiteSpace(mensagemPersonalizada)
                ? "Este e um e-mail de diagnostico para validar a configuracao de envio do Sistema Atron."
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
            <h1>Diagnostico de E-mail</h1>
        </div>
        <div class='content'>
            <p>{mensagem}</p>
            <div class='info-box'>
                <p><strong>Provider:</strong> {ObterProviderConfigurado()}</p>
                <p><strong>Host:</strong> {ObterHostStatus()}</p>
                <p><strong>Remetente:</strong> {_settings.FromEmail}</p>
                <p><strong>Data/Hora:</strong> {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
            </div>
            <p>Se voce recebeu este e-mail, o servico de e-mail esta funcionando corretamente.</p>
        </div>
        <div class='footer'>
            <p>Modulo AtronEmail - Diagnostico Interno</p>
            <p>&copy; {DateTime.Now.Year} Sistema Atron</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}
