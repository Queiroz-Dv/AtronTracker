using Shared.DTO;

namespace Communication.Interfaces.Services
{
    public interface IRouterBuilderService
    {
        public abstract string BuildRoute(string protocolo, string url);

        public abstract void BuildUrl(string route, PageRequestInfoDTO requestInfoDTO);
    }
}