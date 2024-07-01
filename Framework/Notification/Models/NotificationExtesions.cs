using Notification.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Notification.Models
{
    public static class NotificationExtesions
    {
        public static bool HasErrors(this IList<NotificationMessage> messages)
        {
            return messages.Count(m => m.Type == ENotificationType.Error) > 0;
        }
    }
}