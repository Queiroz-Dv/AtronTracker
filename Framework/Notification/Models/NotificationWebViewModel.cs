using Notification.Services;
using System;

namespace Notification.Models
{
    [Serializable]
    public abstract class NotificationWebViewModel : NotificationWebViewService
    {
        const string ERROR = "Error";
        const string WARNING = "Warning";
        const string MESSAGE = "Message";

        public override void AddApiError(string message)
        {
            AddApiNotification(message, ERROR);
        }

        public override void AddApiMessage(string message)
        {
            AddApiNotification(message, MESSAGE);
        }

        public override void AddApiWarning(string message)
        {
            AddApiNotification(message, WARNING);
        }      
    }
}