using Shared.Enums;

namespace Shared.DTO
{
    [Serializable]
    public class ResultResponse
    {
        public string Message { get; set; }

        public string Level { get; set; }

        public ResultResponseLevelEnum MessageLevel {get; set;}
    }
}