using System.ComponentModel;

namespace Shared.Application.DTOS.Requests
{
    /// <summary>
    /// Request de envio de e-mail.
    /// </summary>
    public class EmailRequest
    {
        /// <summary>
        /// E-mail de destino 
        /// </summary>
        [DisplayName("E-mails de destino")]
        public List<string> EmailsDestino { get; set; }

        /// <summary>
        /// Assunto do e-mail
        /// Se não informado, será usado um assunto padrão.
        /// </summary>
        public string Assunto { get; set; }

        /// <summary>
        /// Mensagem personalizada no corpo do e-mail
        /// </summary>
        public string Mensagem { get; set; }
    }
}
