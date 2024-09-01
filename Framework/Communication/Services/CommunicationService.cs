using Communication.Interfaces.Services;
using Shared.DTO;

namespace Communication.Services
{
    /// <summary>
    /// Classe que implementa os processos de alimentação da lista dos resultados da resposta da API
    /// </summary>
    public class CommunicationService : ICommunicationService
    {
        private readonly List<ResultResponseDTO> responses = new List<ResultResponseDTO>();

        public void AddResponseContent(ResultResponseDTO resultResponse)
        {
            responses.Add(resultResponse);
        }

        public void AddResponseContent(List<ResultResponseDTO> resultResponses)
        {
            responses.AddRange(resultResponses);
        }

        public List<ResultResponseDTO> GetResultResponses()
        {
            return responses;
        }
    }
}