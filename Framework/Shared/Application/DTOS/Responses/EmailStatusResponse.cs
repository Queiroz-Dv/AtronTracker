namespace Shared.Application.DTOS.Responses
{
    /// <summary>
    /// Response do status/diagnóstico do serviço de e-mail.
    /// </summary>
    public class EmailStatusResponse
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida.
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem de resultado da operação.
        /// </summary>
        public string Mensagem { get; set; } = null!;

        /// <summary>
        /// Provedor de e-mail identificado.
        /// </summary>
        public string? ProvedorIdentificado { get; set; }

        /// <summary>
        /// Host SMTP utilizado.
        /// </summary>
        public string? SmtpHost { get; set; }

        /// <summary>
        /// Porta SMTP utilizada.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// E-mail do remetente configurado.
        /// </summary>
        public string? EmailRemetente { get; set; }

        /// <summary>
        /// E-mail de destino do diagnóstico.
        /// </summary>
        public List<string> EmailDestino { get; set; } = [];

        /// <summary>
        /// Data/hora da operação.
        /// </summary>
        public DateTime? DataOperacao { get; set; }

        /// <summary>
        /// Detalhes do erro, se houver.
        /// </summary>
        public string? ErroDetalhes { get; set; }

        /// <summary>
        /// Indica se o serviço está operacional.
        /// </summary>
        public bool ServicoOperacional { get; set; }

        /// <summary>
        /// Cria uma resposta de sucesso para envio de diagnóstico.
        /// </summary>
        public static EmailStatusResponse CriarSucesso(
            string provedor,
            string smtpHost,
            int smtpPort,
            string emailRemetente,
            List<string> emailDestino)
        {
            return new EmailStatusResponse
            {
                Sucesso = true,
                ServicoOperacional = true,
                Mensagem = "E-mail de diagnóstico enviado com sucesso!",
                ProvedorIdentificado = provedor,
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                EmailRemetente = emailRemetente,
                EmailDestino = emailDestino,
                DataOperacao = DateTime.Now
            };
        }

        /// <summary>
        /// Cria uma resposta de erro.
        /// </summary>
        public static EmailStatusResponse CriarErro(string mensagem, string? detalhes = null)
        {
            return new EmailStatusResponse
            {
                Sucesso = false,
                ServicoOperacional = false,
                Mensagem = mensagem,
                ErroDetalhes = detalhes,
                DataOperacao = DateTime.Now
            };
        }

        /// <summary>
        /// Cria uma resposta de status do serviço.
        /// </summary>
        public static EmailStatusResponse CriarStatus(
            bool operacional,
            string provedor,
            string smtpHost,
            int smtpPort,
            string emailRemetente)
        {
            return new EmailStatusResponse
            {
                Sucesso = operacional,
                ServicoOperacional = operacional,
                Mensagem = operacional ? "Serviço de e-mail operacional." : "Serviço de e-mail não operacional.",
                ProvedorIdentificado = provedor,
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                EmailRemetente = emailRemetente,
                DataOperacao = DateTime.Now
            };
        }
    }
}
