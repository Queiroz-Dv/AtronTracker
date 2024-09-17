using Communication.Interfaces.Services;
using Shared.DTO;
using Shared.Models;

namespace Communication.Services
{
    /// <summary>
    /// Classe que implementa os processos de alimentação da lista dos resultados da resposta da API
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly List<ResultResponseDTO> responses = new List<ResultResponseDTO>();

        public List<Message> Messages { get; set; } = new List<Message>();

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }

        public void AddMessages(List<Message> messages)
        {
            Messages.AddRange(messages);
        }

        public void AddResponseContent(ResultResponseDTO resultResponse)
        {
            responses.Add(resultResponse);
        }

        public void AddResponseContent(List<ResultResponseDTO> resultResponses)
        {
            responses.AddRange(resultResponses);
        }

        public List<Message> GetMessages()
        {
            return Messages;
        }

        public List<ResultResponseDTO> GetResultResponses()
        {
            return responses;
        }
    }
}