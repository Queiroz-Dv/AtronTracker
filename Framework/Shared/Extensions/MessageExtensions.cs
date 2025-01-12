using Shared.Enums;
using Shared.Models;

namespace Shared.Extensions
{
    public static class MessageExtensions
    {
        public static bool HasErrors(this IList<Message> messages)
        {
            return messages.Any(m => m.Level == MessageLevel.Error);
        }

        //TODO: Buscar uma forma de fazer a conversão para um formato JSON válido ao invés de um dynamic
        public static IEnumerable<dynamic> ConvertMessageToJson(this IList<Message> messages)
        {
            var message = from ms in messages
                          select new
                          {
                              ms.Description,
                              Level = ms.Level.GetEnumDescription(),
                          };

            return message;
        }
    }
}