using Notification.Models;
using System.Collections.Generic;

namespace Notification.Interfaces.DTO
{
    /// <summary>
    /// Interface de acesso as notificações
    /// </summary>
    public interface INotificationMessage
    {
        /// <summary>
        /// Lista de mensagens para comunicação entre os módulos
        /// </summary>
        public List<NotificationMessage> Messages { get; }
    }
}