using Communication.Interfaces;
using Communication.Interfaces.Services;

namespace Communication.Services
{
    public class RouterBuilder : IRouterBuilderService
    {
        private readonly IApiClient _apiClient;

        public RouterBuilder(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        //public string BuildRoute(string protocolo, string url)
        //{
        //    return $"{protocolo}{url}/";
        //}

        //public void BuildUrl(string route, PageRequestInfoDTO requestInfoDTO)
        //{
        //    var url = requestInfoDTO.Parameter.IsNullOrEmpty() ? $"{route}{requestInfoDTO.ApiControllerName}/{requestInfoDTO.ApiControllerAction}" :
        //                                            $"{route}{requestInfoDTO.ApiControllerName}/{requestInfoDTO.ApiControllerAction}/{requestInfoDTO.Parameter}";

        //    _apiClient.Url = url;
        //}

        public void TransferRouteToApiClient(string url)
        {
            _apiClient.Url = url;
        }
    }
}