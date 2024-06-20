using Helpers.Interfaces;
using Notification.Enums;
using Notification.Interfaces;
using Notification.Models;

namespace Helper.Objects
{
    public abstract class ValidateHelper<Model> : IValidateHelper<Model>
    {
        public INotificationService NotificationService { get; set; }

        public ValidateHelper(INotificationService notificationService)
        {
            NotificationService = notificationService;
        }

        public abstract void Validate(Model entity);

        protected void AddRequiredFieldMessage(string fieldName)
        {
            var notificationMessage = new NotificationMessage(string.Format("The field {0} is required", fieldName.ToLower()), ENotificationType.Error);
            NotificationService.Messages.Add(notificationMessage);
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
