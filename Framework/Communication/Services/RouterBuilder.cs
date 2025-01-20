using Communication.Interfaces.Services;
using Shared.DTO;
using Shared.Extensions;

namespace Communication.Services
{
    public class RouterBuilder : IRouterBuilderService
    {
        private readonly IUrlTransferService _urlTransferService;

        public RouterBuilder(IUrlTransferService urlTransferService)
        {
            _urlTransferService = urlTransferService;
        }

        public string BuildRoute(string protocolo, string url)
        {
            return $"{protocolo}{url}/";
        }

        public void BuildUrl(string route, PageRequestInfoDTO requestInfoDTO)
        {
            var url = requestInfoDTO.Parameter.IsNullOrEmpty() ? $"{route}{requestInfoDTO.ApiControllerName}/{requestInfoDTO.ApiControllerAction}" :
                                                    $"{route}{requestInfoDTO.ApiControllerName}/{requestInfoDTO.ApiControllerAction}/{requestInfoDTO.Parameter}";
            _urlTransferService.Url = url;
        }
    }
}