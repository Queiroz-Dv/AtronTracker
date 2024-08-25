using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Shared.Enums;

namespace Shared.DTO
{
    [Serializable]
    public class ResultResponseDTO
    {        
        public string Message { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public ResultResponseLevelEnum Level {get; set;}
    }
}