using PersonalTracking.Notification.Models;
using System.Collections.Generic;

namespace PersonalTracking.Notification.Interfaces
{
    public interface INotificationService
    {
        List<NotificationMessage> Messages { get; set; }

        void AddMessage(string message);

        void AddError(string message);

        void AddWarning(string message);
    }
}