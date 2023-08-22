using PersonalTracking.Helper.Helpers;
using PersonalTracking.Models;

namespace BLL.Validation
{
    public class DepartmentValidationService : ValidateHelper<DepartmentModel>
    {
        public override void Validate(DepartmentModel entity)
        {
            if (string.IsNullOrEmpty(entity.DepartmentModelName))
            {
                notificationService.AddError("Department name is empty.");
            }
        }
    }
}
