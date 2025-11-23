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

        public void TransferRouteToApiClient(string url)
        {
            _apiClient.Url = url;
        }
    }
}