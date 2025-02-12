using Shared.Enums;
using Shared.Models;
using System.Text.Json;

namespace Shared.Extensions
{
    public static class MessageExtensions
    {
        public static bool HasErrors(this IList<Message> messages)
        {
            return messages.Any(m => m.Level == MessageLevel.Error);
        }

        public static IEnumerable<string> ConvertMessageToJson(this IList<Message> messages)
        {
            return new List<string> { JsonSerializer.Serialize(messages.Select(ms => new
            {
                ms.Description,
                Level = ms.Level.GetEnumDescription(),
            }), new JsonSerializerOptions { WriteIndented = true }) };
        }
    }
}