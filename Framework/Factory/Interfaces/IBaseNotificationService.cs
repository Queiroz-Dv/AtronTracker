using Notification.Models;
using System.Collections.Generic;

namespace Factory.Interfaces
{
    public interface IBaseNotificationService
    {
        List<NotificationMessage> GetNotifications();
    }
}