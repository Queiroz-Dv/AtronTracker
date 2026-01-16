namespace Shared.Application.Email
{
    /// <summary>
    /// Classe responsável por identificar provedores de e-mail e fornecer suas configurações SMTP.
    /// </summary>
    public static class EmailProviderIdentifier
    {
        /// <summary>
        /// Mapeamento de domínios para provedores.
        /// </summary>
        private static readonly Dictionary<string, EmailProvider> DomainToProvider = new(StringComparer.OrdinalIgnoreCase)
        {
            // Gmail
            { "gmail.com", EmailProvider.Gmail },
            { "googlemail.com", EmailProvider.Gmail },
            
            // Outlook/Microsoft
            { "outlook.com", EmailProvider.Outlook },
            { "outlook.com.br", EmailProvider.Outlook },
            { "hotmail.com", EmailProvider.Outlook },
            { "hotmail.com.br", EmailProvider.Outlook },
            { "live.com", EmailProvider.Outlook },
            { "msn.com", EmailProvider.Outlook },
            
            // Yahoo
            { "yahoo.com", EmailProvider.Yahoo },
            { "yahoo.com.br", EmailProvider.Yahoo },
            { "ymail.com", EmailProvider.Yahoo }
        };

        /// <summary>
        /// Configurações SMTP para cada provedor.
        /// </summary>
        private static readonly Dictionary<EmailProvider, EmailProviderSettings> ProviderSettings = new()
        {
            { EmailProvider.Gmail, new EmailProviderSettings("smtp.gmail.com", 587) },
            { EmailProvider.Outlook, new EmailProviderSettings("smtp-mail.outlook.com", 587) },
            { EmailProvider.Yahoo, new EmailProviderSettings("smtp.mail.yahoo.com", 587) }
        };

        /// <summary>
        /// Identifica o provedor de e-mail a partir do endereço de e-mail.
        /// </summary>
        public static EmailProvider IdentificarProvedor(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("O e-mail não pode ser nulo ou vazio.", nameof(email));
            }

            var partes = email.Split('@');
            if (partes.Length != 2)
            {
                throw new ArgumentException("Formato de e-mail inválido.", nameof(email));
            }

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
                throw new NotSupportedException(
                    "Provedor de e-mail não suportado. " +
                    "Os provedores suportados são: Gmail, Outlook (Hotmail/Live/MSN) e Yahoo.");
            }

            if (!ProviderSettings.TryGetValue(provider, out var settings))
            {
                throw new NotSupportedException($"Configurações não encontradas para o provedor: {provider}");
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
