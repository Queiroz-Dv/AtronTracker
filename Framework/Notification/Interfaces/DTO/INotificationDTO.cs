using Notification.Models;
using Shared;
using System.Collections.Generic;

namespace Notification.Interfaces.DTO
{
    public interface INotificationDTO : ITransactionResponseClient
    {
        public List<NotificationMessage> _messages { get; }
    }
}