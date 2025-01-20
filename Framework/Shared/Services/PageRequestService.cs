using Shared.DTO;
using Shared.Interfaces;

namespace Shared.Services
{
    public class PageRequestService : IPageRequestService
    {
        private PageRequestInfoDTO PageRequestInfoDTO { get; set; }
        
        public void ConfigurePageRequestInfo(string apiControllerName, string apiControllerAction = "", string filter = "")
        {
            var pageRequestInfo = new PageRequestInfoDTO()
            {
                ApiControllerName = apiControllerName,
                ApiControllerAction = apiControllerAction,
                Filter = filter
            };

            PageRequestInfoDTO = pageRequestInfo;
        }

        public PageRequestInfoDTO GetPageRequestInfo()
        {
            return PageRequestInfoDTO;
        }
    }
}