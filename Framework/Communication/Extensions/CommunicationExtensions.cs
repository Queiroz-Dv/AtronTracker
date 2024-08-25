using Shared.DTO;
using Shared.Enums;

namespace Communication.Extensions
{
    public static class CommunicationExtensions
    {
        public static bool HasErrors(this List<ResultResponseDTO> resultResponses)
        {
            return resultResponses.Any(rst => rst.Level == ResultResponseLevelEnum.Error);
        }
    }
}