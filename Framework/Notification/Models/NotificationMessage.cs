using Notification.Enums;
using System;
using System.Text.Json.Serialization;

namespace Notification.Models
{
    [Serializable]
    public class NotificationMessage
    {
        public NotificationMessage(string message, ENotificationType type)
        {
            Message = message;
            Type = type;
            Level = GetLevelMessage(type);
        }

        public NotificationMessage(string message)
        {
            Message = message;
            Type = ENotificationType.Message;
            Level = GetLevelMessage();
        }

        public string Message { get; set; }

        [JsonIgnore]
        public ENotificationType Type { get; set; }

        public string Level { get; set; }

        private string GetLevelMessage()
        {
            switch (Type)
            {
                case ENotificationType.Message: return "Message";
                case ENotificationType.Error: return "Error";
                case ENotificationType.Warning: return "Warning";
                default: throw new ArgumentException("Invalid level");
            }
        }

        private string GetLevelMessage(ENotificationType type)
        {
            switch (type)
            {
                case ENotificationType.Message: return "Message";
                case ENotificationType.Error: return "Error";
                case ENotificationType.Warning: return "Warning";
                default: throw new ArgumentException("Invalid level");
            }
        }
    }
}