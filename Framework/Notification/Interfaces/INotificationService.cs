using Notification.Models;
using System.Collections.Generic;

namespace Notification.Interfaces
{
    public interface INotificationService
    {
        List<NotificationMessage> Messages { get; }

        void AddMessage(string message);

        void AddError(string message);

        void AddWarning(string message);
    }
}