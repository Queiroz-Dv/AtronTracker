using Notification.Interfaces.DTO;
using Shared.DTO;
using System.Collections.Generic;

namespace Notification.Interfaces
{
    public interface INotificationWebViewService
    {
        List<ApiWebViewMessageResponse> MessageResponses { get; }
    }
}