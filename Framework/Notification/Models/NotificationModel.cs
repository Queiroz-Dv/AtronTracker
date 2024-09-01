using Notification.Services;
using System;

namespace Notification.Models
{
    /// <summary>
    /// Classe de modelo de validação e notificações
    /// </summary>
    /// <typeparam name="Entity">Entidade que será utilizada como modelo</typeparam>
    [Serializable]
    public abstract class NotificationModel<Entity> : NotificationService
    {
        public override void AddError(string message)
        {
            AddNotification(message, Enums.ENotificationType.Error);
        }

        public override void AddMessage(string message)
        {
            AddNotification(message, Enums.ENotificationType.Message);
        }

        public override void AddWarning(string message)
        {
            AddNotification(message, Enums.ENotificationType.Warning);
        }

        /// <summary>
        /// Método que realiza as validações da entidade
        /// </summary>
        /// <param name="entity">Entidade que será validada</param>
        public abstract void Validate(Entity entity);
    }
}