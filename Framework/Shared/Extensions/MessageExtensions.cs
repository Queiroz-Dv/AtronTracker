using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace Shared.Extensions
{
    public static class MessageExtensions
    {
        public static bool HasErrors(this IList<NotificationMessage> messages)
        {
            return messages.Any(m => m.Nivel == ENotificationType.Error);
        }

        public static bool TemErros(this IList<NotificationMessage> messages)
        {
            return messages.Any(m => m.Nivel == ENotificationType.Error);
        }


        public static IEnumerable<object> ConvertMessageToJson(this IList<NotificationMessage> messages)
        {
            return messages.Select(ms => new
            {
                ms.Descricao,
                ms.Nivel
            });
        }
    }
}