namespace AtronEmail.DTOs.Requests
{
    /// <summary>
    /// Request para diagnóstico de envio de e-mail.
    /// </summary>
    public class EmailDiagnosticoRequest
    {
        /// <summary>
        /// E-mail de destino do diagnóstico.
        /// </summary>
        public string EmailDestino { get; set; } = null!;

        /// <summary>
        /// Assunto do e-mail de diagnóstico.
        /// Se não informado, será usado um assunto padrão.
        /// </summary>
        public string? Assunto { get; set; }

        /// <summary>
        /// Mensagem personalizada no corpo do e-mail.
        /// </summary>
        public string? Mensagem { get; set; }
    }
}
