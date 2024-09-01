using Notification.Enums;
using Notification.Interfaces;
using Notification.Models;
using System.Collections.Generic;

namespace Notification.Services
{
    /// <summary>
    /// Classe de implementação dos serviços de notificações
    /// </summary>
    public abstract class NotificationService : INotificationService
    {
        protected NotificationService()
        {
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; }

        /// <summary>
        /// Método de automação para inclusão de notificações
        /// </summary>
        /// <param name="message">Mensagem de notificação</param>
        /// <param name="type">Tipo de notificação</param>
        public void AddNotification(string message, ENotificationType type)
        {
            Messages.Add(new NotificationMessage(message, type));
        }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}