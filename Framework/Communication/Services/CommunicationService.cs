using Communication.Interfaces.Services;
using Shared.DTO;

namespace Communication.Services
{
    public class CommunicationService : ICommunicationService
    {
        private readonly List<ResultResponse> responses = new List<ResultResponse>();

        public void AddResponseContent(ResultResponse resultResponse)
        {
            responses.Add(resultResponse);
        }

        public void AddResponseContent(List<ResultResponse> resultResponses)
        {
            responses.AddRange(resultResponses);
        }

        public List<ResultResponse> GetResultResponses()
        {
            return responses;
        }
    }
}