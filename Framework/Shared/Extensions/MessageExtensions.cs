using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace Shared.Extensions
{
    public static class MessageExtensions
    {       
        public static bool TemErros(this IEnumerable<NotificationMessage> messages)
        {
            return messages.Any(m => m.Nivel == ENotificationType.Error);
        }       
    }
}
