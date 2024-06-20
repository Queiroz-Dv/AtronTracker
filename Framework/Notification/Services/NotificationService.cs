using Notification.Enums;
using Notification.Interfaces;
using Notification.Models;
using System.Collections.Generic;

namespace Notification.Services
{
    public abstract class NotificationService : INotificationService
    {
        protected NotificationService()
        {
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; }

        public void AddNotification(string message, ENotificationType type)
        {
            Messages.Add(new NotificationMessage(message, type));
        }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}