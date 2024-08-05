using Shared.DTO;

namespace Communication.Interfaces.Services
{
    public interface ICommunicationService
    {
        void AddResponseContent(ResultResponse resultResponse);
        void AddResponseContent(List<ResultResponse> resultResponses);
        List<ResultResponse> GetResultResponses();
    }
}
