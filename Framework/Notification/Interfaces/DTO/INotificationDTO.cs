using Notification.Models;
using System.Collections.Generic;

namespace Notification.Interfaces.DTO
{
    public interface INotificationDTO
    {
        public List<NotificationMessage> Messages { get; }
    }
}