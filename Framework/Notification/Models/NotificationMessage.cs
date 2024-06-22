using Notification.Enums;
using System;

namespace Notification.Models
{
    [Serializable]
    public class NotificationMessage
    {
        public NotificationMessage(string message, ENotificationType type)
        {
            Message = message;
            Type = type;
        }

        public NotificationMessage(string message)
        {
            Message = message;
            Type = ENotificationType.Message;
        }

        public string Message { get; set; }

        public ENotificationType Type { get; set; }
    }
}