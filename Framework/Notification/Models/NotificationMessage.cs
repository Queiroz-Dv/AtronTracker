using Notification.Enums;

namespace Notification.Models
{
    public class NotificationMessage
    {
        public NotificationMessage(string message, ENotificationType type)
        {
            Message = message;
            Type = type;
            ErrorLevel = (int)type;
            SetTypeMessage(type);
        }

        private void SetTypeMessage(ENotificationType type)
        {
            switch (type)
            {
                case ENotificationType.Error:
                    TypeMessage = "Error";
                    break;
                case ENotificationType.Warning:
                    TypeMessage = "Warning";
                    break;
                case ENotificationType.Message:
                    TypeMessage = "Message";
                    break;
                default:
                    break;
            }
        }

        public string Message { get; set; }

        public string Detail { get; set; }

        public ENotificationType Type { get; set; }

        public string TypeMessage { get; set; }

        public int ErrorLevel { get; set; }
    }
}
