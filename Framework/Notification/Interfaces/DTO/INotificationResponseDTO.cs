using Shared.DTO;

namespace Notification.Interfaces.DTO
{
    public interface INotificationResponseDTO
    {
        ApiWebViewMessageResponse GetJsonResponseContent();
    }
}