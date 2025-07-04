namespace Shared.DTO.Email
{
    // DTO de configuração de SMTP
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = null!;
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FromName { get; set; } = "Atron System";
        public string FromEmail { get; set; } = null!;
    }
}