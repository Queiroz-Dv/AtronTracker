using Newtonsoft.Json.Converters;
using Shared.Enums;
using Shared.Interfaces;
using System.Text.Json.Serialization;

namespace Shared.Models
{
    [Serializable]
    public class Message
    {
        public string Description { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MessageLevel Level { get; set; }
    }
}
