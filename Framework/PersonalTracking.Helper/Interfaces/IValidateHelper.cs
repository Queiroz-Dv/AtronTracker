using PersonalTracking.Notification.Interfaces;

namespace PersonalTracking.Helper.Interfaces
{
    public interface IValidateHelper<TModel> 
    {
        INotificationService NotificationService { get; set; }

        void Validate(TModel entity);
    }
}