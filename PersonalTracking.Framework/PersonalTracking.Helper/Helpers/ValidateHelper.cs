using PersonalTracking.Notification.Interfaces;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Notification.Enums;
using PersonalTracking.Notification.Models;

namespace PersonalTracking.Helper.Helpers
{
    public abstract class ValidateHelper<TEntity> : IValidateHelper<TEntity>
    {
        public INotificationService notificationService;

        public abstract void Validate(TEntity condition);

        protected void AddRequiredFieldMessage(string fieldName)
        {
            var notificationMessage = new NotificationMessage(string.Format("The field {0} is required", fieldName.ToLower()), ENotificationType.Error);
            notificationService.Messages.Add(notificationMessage);
        }

        protected void AddRequiredFieldMessage(bool condition, string fieldName)
        {
            if (condition)
            {
                AddRequiredFieldMessage(fieldName);
            }
        }
    }
}
