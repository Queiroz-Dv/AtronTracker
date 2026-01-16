using Shared.Application.DTOS.Email;

namespace AtronEmail.Application.Interfaces
{
    /// <summary>
    /// Interface do serviço de notificação por e-mail.
    /// Integra com o padrão de notificações para enviar e-mails a usuários superiores.
    /// </summary>
    public interface IEmailNotificationService
    {
        /// <summary>
        /// Envia e-mail de notificação para uma lista de destinatários.
        /// </summary>
        /// <param name="assunto">Assunto do e-mail.</param>
        /// <param name="mensagem">Corpo da mensagem (HTML suportado).</param>
        /// <param name="destinatarios">Lista de e-mails dos destinatários.</param>
        Task EnviarNotificacaoAsync(string assunto, string mensagem, IEnumerable<string> destinatarios);

        /// <summary>
        /// Envia e-mail de notificação para superiores sobre uma alteração crítica.
        /// </summary>
        /// <param name="tipoEvento">Tipo do evento (ex: "Alteração de Salário", "Novo Usuário").</param>
        /// <param name="descricao">Descrição detalhada do evento.</param>
        /// <param name="entidade">Nome da entidade afetada.</param>
        /// <param name="emailsSuperiores">Lista de e-mails dos superiores.</param>
        Task NotificarSuperioresAsync(string tipoEvento, string descricao, string entidade, IEnumerable<string> emailsSuperiores);

        /// <summary>
        /// Envia e-mail usando a mensagem padrão do sistema.
        /// </summary>
        /// <param name="message">Mensagem de e-mail.</param>
        Task EnviarAsync(EmailMessage message);
    }
}
