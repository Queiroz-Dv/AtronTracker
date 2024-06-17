using Notification.Interfaces;
using Notification.Enums;
using Notification.Models;
using System.Collections.Generic;

namespace Notification.Services
{
    public abstract class NotificationService : INotificationService
    {
        public List<NotificationMessage> Messages { get; set; }

        public  void AddNotification(string message, ENotificationType type)
        {
            Messages.Add(new NotificationMessage(message, type));
        }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}