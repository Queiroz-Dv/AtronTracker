using PersonalTracking.Models;
using PersonalTracking.Notification.Models;
using System.Collections.Generic;

namespace BLL.Interfaces
{
    public interface IDepartmentService : IGenericServices<DepartmentModel>
    {
        List<NotificationMessage> Messages { get; set; }
    }
}