using Notification.Services;
using System;

namespace Notification.Models
{
    [Serializable]
    public abstract class NotificationModel<Entity> : NotificationService
    {

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

        public abstract void Validate(Entity entity);
    }
}