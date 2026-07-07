namespace Shared.Application.DTOS.Email
{
    // DTO de configuração de SMTP
    public class EmailSettings
    {
        public string Provider { get; set; } = "Smtp";
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FromName { get; set; }
        public string FromEmail { get; set; } = null!;
        public BrevoEmailSettings Brevo { get; set; } = new();
    }

    public class BrevoEmailSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://api.brevo.com/v3";
    }
}
