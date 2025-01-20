using Shared.DTO;

namespace Shared.Interfaces
{
    public interface IPageRequestService
    {
        void ConfigurePageRequestInfo(string apiControllerName, string apiControllerAction = "", string filter = "");

        PageRequestInfoDTO GetPageRequestInfo();
    }
}