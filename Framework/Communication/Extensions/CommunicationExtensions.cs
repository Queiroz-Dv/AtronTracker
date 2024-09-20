using Communication.Interfaces.Services;
using Shared.DTO;
using Shared.Enums;
using Shared.Models;

namespace Communication.Extensions
{
    /// <summary>
    /// Classe de métodos extendidos para gerenciamento da lista de resultados das respostas da API
    /// </summary>
    public static class CommunicationExtensions
    {
        /// <summary>
        /// Método que analisa se tem algum erro 
        /// </summary>
        /// <param name="resultResponses"></param>
        public static bool HasErrors(this List<ResultResponseDTO> resultResponses)
        {
            return resultResponses.Any(rst => rst.Level == ResultResponseLevelEnum.Error);
        }

        public static void FillMessages(this List<Message> messages, ICommunicationService communicationService)
        {
            if (communicationService.Messages != null)
            {
                messages.AddRange(communicationService.Messages);
            }
        }
    }
}