using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Http;

namespace Atron.Application.ApiServices
{
    public class ApiRouteService : IApiRouteService
    {
        private IHttpContextAccessor _contextAccessor;

        public ApiRouteService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        private string UrlBase()
        {
            var request = _contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host.Value}/api";
            return baseUrl;
        }

        public ApiRoute ObterRotaPorModulo(string modulo)
        {
            var baseUrl = UrlBase();

            var route = new ApiRoute()
            {
                Url = baseUrl,
                Modulo = modulo
            };

            return route;
        }
    }
}