using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Email
{
    /// <summary>
    /// Classe responsável por identificar provedores de e-mail e fornecer suas configurações SMTP.
    /// </summary>
    public static class EmailProviderIdentifier
    {
    public static readonly List<NotificationMessage> Messages = new List<NotificationMessage>();

        /// <summary>
        /// Mapeamento de domínios para provedores.
        /// </summary>
        private static readonly Dictionary<string, EmailProvider> DomainToProvider = new(StringComparer.OrdinalIgnoreCase)
        {
            // Gmail
            { EmailResource.GmailProvider, EmailProvider.Gmail },
            { EmailResource.GoogleMailProvider, EmailProvider.Gmail },

            // Outlook / Microsoft family
            { EmailResource.OutlookProvider, EmailProvider.Outlook },
            { EmailResource.OutlookBrProvider, EmailProvider.Outlook },
            { EmailResource.HotmailProvider, EmailProvider.Outlook },
            { EmailResource.HotmailBrProvider, EmailProvider.Outlook },
            { EmailResource.LiveProvider, EmailProvider.Outlook },
            { EmailResource.MsnProvider, EmailProvider.Outlook },

            // Yahoo family
            { EmailResource.YahooProvider, EmailProvider.Yahoo },
            { EmailResource.YahooBrProvider, EmailProvider.Yahoo },
            { EmailResource.YahooMailProvider, EmailProvider.Yahoo }
        };

        /// <summary>
        /// Configurações SMTP para cada provedor.
        /// </summary>
        private static readonly Dictionary<EmailProvider, EmailProviderSettings> ProviderSettings = new()
        {
            { EmailProvider.Gmail, new EmailProviderSettings(EmailResource.GoogleSmtp) },
            { EmailProvider.Outlook, new EmailProviderSettings(EmailResource.OutlookSmtp) },
            { EmailProvider.Yahoo, new EmailProviderSettings(EmailResource.YahooSmtp) }
        };

        /// <summary>
        /// Identifica o provedor de e-mail a partir do endereço de e-mail.
        /// </summary>
        public static EmailProvider IdentificarProvedor(string email)
        {
            var partes = email.Split('@');
            var dominio = partes[1].ToLowerInvariant();

            return DomainToProvider.TryGetValue(dominio, out var provider)
                ? provider
                : EmailProvider.Desconhecido;
        }

        /// <summary>
        /// Obtém as configurações SMTP para o provedor especificado.
        /// </summary>
        public static EmailProviderSettings ObterConfiguracoes(EmailProvider provider)
        {
            if (provider == EmailProvider.Desconhecido)
            {                         
                Messages.Add(new NotificationMessage()
                {
                    Descricao = EmailResource.ErroProvedorDesconhecido,
                    Nivel = ENotificationType.Error
                });
            }

            if (!ProviderSettings.TryGetValue(provider, out var settings))
            {
                Messages.Add(new NotificationMessage()
                {
                    Descricao = string.Format(EmailResource.ErroConfiguracoesProvedor, provider),
                    Nivel = ENotificationType.Error
                });
            }

            return settings;
        }

        /// <summary>
        /// Identifica o provedor e retorna suas configurações em uma única operação.
        /// </summary>
        public static (EmailProvider Provider, EmailProviderSettings Settings) IdentificarEObterConfiguracoes(string email)
        {
            var provider = IdentificarProvedor(email);
            var settings = ObterConfiguracoes(provider);
            return (provider, settings);
        }
    }
}
