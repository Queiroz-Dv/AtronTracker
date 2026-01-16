using System;

namespace Application.DTO.Response
{
    /// <summary>
    /// Response do teste de envio de e-mail.
    /// </summary>
    public class EmailTesteResponse
    {
        /// <summary>
        /// Indica se o teste foi bem-sucedido.
        /// </summary>
        public bool Sucesso { get; set; }

        /// <summary>
        /// Mensagem de resultado do teste.
        /// </summary>
        public string Mensagem { get; set; }

        /// <summary>
        /// Provedor de e-mail identificado.
        /// </summary>
        public string ProvedorIdentificado { get; set; }

        /// <summary>
        /// Host SMTP utilizado.
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>
        /// Porta SMTP utilizada.
        /// </summary>
        public int SmtpPort { get; set; }

        /// <summary>
        /// E-mail do remetente configurado.
        /// </summary>
        public string EmailRemetente { get; set; }

        /// <summary>
        /// E-mail de destino do teste.
        /// </summary>
        public string EmailDestino { get; set; }

        /// <summary>
        /// Data/hora do envio.
        /// </summary>
        public DateTime? DataEnvio { get; set; }

        /// <summary>
        /// Detalhes do erro, se houver.
        /// </summary>
        public string ErroDetalhes { get; set; }

        /// <summary>
        /// Cria uma resposta de sucesso.
        /// </summary>
        public static EmailTesteResponse CriarSucesso(
            string provedor, 
            string smtpHost, 
            int smtpPort, 
            string emailRemetente, 
            string emailDestino)
        {
            return new EmailTesteResponse
            {
                Sucesso = true,
                Mensagem = "E-mail de teste enviado com sucesso!",
                ProvedorIdentificado = provedor,
                SmtpHost = smtpHost,
                SmtpPort = smtpPort,
                EmailRemetente = emailRemetente,
                EmailDestino = emailDestino,
                DataEnvio = DateTime.Now
            };
        }

        /// <summary>
        /// Cria uma resposta de erro.
        /// </summary>
        public static EmailTesteResponse CriarErro(string mensagem, string detalhes = null)
        {
            return new EmailTesteResponse
            {
                Sucesso = false,
                Mensagem = mensagem,
                ErroDetalhes = detalhes,
                DataEnvio = DateTime.Now
            };
        }
    }
}
