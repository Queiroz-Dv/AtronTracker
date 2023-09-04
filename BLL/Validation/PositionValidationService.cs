using PersonalTracking.Helper.Helpers;
using PersonalTracking.Models;
using PersonalTracking.Notification.Interfaces;

namespace BLL.Validation
{
    public class PositionValidationService : ValidateHelper<PositionModel>
    {
        public PositionValidationService(INotificationService notificationService) : base(notificationService) { }

        public override void Validate(PositionModel model)
        {
            if (string.IsNullOrEmpty(model.PositionName))
            {
                NotificationService.AddError("Position name is empty");
            }
        }
    }
}
