using PersonalTracking.Helper.Helpers;
using PersonalTracking.Models;
using PersonalTracking.Notification.Interfaces;

namespace BLL.Validation
{
    public class DepartmentValidationService : ValidateHelper<DepartmentModel>
    {
        public DepartmentValidationService(INotificationService notificationService) : base(notificationService) { }

        public override void Validate(DepartmentModel entity)
        {
            if (string.IsNullOrEmpty(entity.DepartmentModelName))
            {
                NotificationService.AddError("Department name is empty.");
            }
        }
    }
}
