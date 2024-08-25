using Shared.DTO;

namespace Communication.Interfaces.Services
{
    public interface ICommunicationService
    {
        void AddResponseContent(ResultResponseDTO resultResponse);
        void AddResponseContent(List<ResultResponseDTO> resultResponses);
        List<ResultResponseDTO> GetResultResponses();
    }
}
