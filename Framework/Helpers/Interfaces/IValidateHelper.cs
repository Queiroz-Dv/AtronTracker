using Notification.Interfaces;

namespace Helpers.Interfaces
{
    public interface IValidateHelper<TModel>
    {
        INotificationService NotificationService { get; set; }

        void Validate(TModel entity);
    }
}