using Notification.Enums;
using Notification.Interfaces;
using Notification.Models;
using System.Collections.Generic;

namespace Notification.Services
{
    /// <summary>
    /// Classe de implementa��o dos servi�os de notifica��es
    /// </summary>
    public abstract class NotificationService : INotificationService
    {
        protected NotificationService()
        {
            Messages = new List<NotificationMessage>();
        }

        public List<NotificationMessage> Messages { get; }

        /// <summary>
        /// M�todo de automa��o para inclus�o de notifica��es
        /// </summary>
        /// <param name="message">Mensagem de notifica��o</param>
        /// <param name="type">Tipo de notifica��o</param>
        public void AddNotification(string message, ENotificationType type)
        {
            Messages.Add(new NotificationMessage(message, type));
        }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}