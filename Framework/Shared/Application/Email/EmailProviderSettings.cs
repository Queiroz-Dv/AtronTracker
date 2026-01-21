namespace Shared.Application.Email
{
    /// <summary>
    /// Configurações SMTP para um provedor de e-mail.
    /// </summary>
    public class EmailProviderSettings
    {
        /// <summary>
        /// Host do servidor SMTP.
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// Porta do servidor SMTP.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// Indica se deve usar SSL/TLS.
        /// </summary>
        public bool UseSSL { get; set; }

        public EmailProviderSettings(string smtpHost, int smtpPort = 587, bool useSSL = true)
        {
            SmtpHost = smtpHost;
            SmtpPort = smtpPort;
            UseSSL = useSSL;
        }
    }
}
