using Notification.Services;
using System.Collections.Generic;

namespace Notification.Models
{
    public class NotificationModel : NotificationService
    {
        public NotificationModel()
        {
            Messages = new List<NotificationMessage>();
        }

        public override void AddError(string message)
        {
            AddNotification(message, Enums.ENotificationType.Error);
        }

        public override void AddMessage(string message)
        {
            AddNotification(message, Enums.ENotificationType.Message);
        }

        public override void AddWarning(string message)
        {
            AddNotification(message, Enums.ENotificationType.Warning);
        }
    }
}
