using NTF.Enums;
using NTF.Models;
using System.Collections.Generic;

namespace NTF.Services
{
    public class NotificationService
    {
        public IList<NotificationMessage> Messages { get; set; }

        private void AddNotification(string message, ENotificationType type)
        {
            Messages.Add(new NotificationMessage(message, type));
        }

        protected void AddMessage(string message)
        {
            AddNotification(message, ENotificationType.Message);
        }

        protected void AddError(string message)
        {
            AddNotification(message, ENotificationType.Error);
        }

        protected void AddWarning(string message)
        {
            AddNotification(message, ENotificationType.Warning);
        }
    }
}