using Helper.Objects;
using Models;
using Notification.Interfaces;

namespace PersonalTracking.BLL.Validation
{
    public class DepartmentValidationService : ValidateHelper<Department>
    {
        public DepartmentValidationService(INotificationService notificationService) : base(notificationService) { }

        public override void Validate(Department entity)
        {
            if (string.IsNullOrEmpty(entity.Name))
            {
                NotificationService.AddError("Department name is empty.");
            }
        }
    }
}
