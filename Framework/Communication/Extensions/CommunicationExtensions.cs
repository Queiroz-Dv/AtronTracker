using Shared.DTO;
using Shared.Enums;
using Shared.Extensions;

namespace Communication.Extensions
{
    public static class CommunicationExtensions
    {
        public static bool HasErrors(this List<ResultResponse> resultResponses)
        {
            return resultResponses.Any(rst => rst.Level == ResultResponseLevelEnum.Error);
        }
    }
}