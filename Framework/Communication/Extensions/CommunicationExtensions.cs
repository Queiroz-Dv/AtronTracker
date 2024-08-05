using Shared.DTO;

namespace Communication.Extensions
{
    public static class CommunicationExtensions
    {
        public static bool HasErrors(this List<ResultResponse> resultResponses)
        {
            var hasErrors = resultResponses.Count(rst => rst.Level == "Error") > 0;
            return hasErrors;
        }
    }
}