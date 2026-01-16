using AtronEmail.DTOs.Requests;
using AtronEmail.DTOs.Responses;

namespace AtronEmail.Application.Interfaces
{
    /// <summary>
    /// Interface do serviço de diagnóstico de e-mail.
    /// Utilizado para verificar configurações e testar envio de e-mails.
    /// </summary>
    public interface IEmailDiagnosticService
    {
        /// <summary>
        /// Testa o envio de e-mail com as configurações atuais.
        /// </summary>
        /// <param name="request">Dados do diagnóstico.</param>
        /// <returns>Resultado do diagnóstico com detalhes da configuração.</returns>
        Task<EmailStatusResponse> EnviarDiagnosticoAsync(EmailDiagnosticoRequest request);

        /// <summary>
        /// Verifica se as configurações de e-mail estão válidas.
        /// </summary>
        /// <returns>Status da configuração de e-mail.</returns>
        Task<EmailStatusResponse> VerificarConfiguracaoAsync();

        /// <summary>
        /// Obtém o status atual do serviço de e-mail.
        /// </summary>
        /// <returns>Status resumido do serviço.</returns>
        Task<EmailStatusResponse> ObterStatusAsync();
    }
}
