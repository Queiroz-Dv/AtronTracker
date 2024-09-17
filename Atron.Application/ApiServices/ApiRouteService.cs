using Atron.Application.ApiInterfaces;
using Atron.Domain.ApiEntities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Atron.Application.ApiServices
{
    public class ApiRouteService : IApiRouteService
    {
        private IHttpContextAccessor _contextAccessor;

        public ApiRouteService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public List<ApiRoute> MontarRotasPorModuloService(string modulo)
        {
            string baseUrl = UrlBase();

            var rotasMontadas = MontarRotasPadronizadas(baseUrl, modulo);
            return rotasMontadas;
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

        private List<ApiRoute> MontarRotasPadronizadas(string baseUrl, string modulo)
        {
            var rotasPadronizadas = new List<ApiRoute>()
            {
                new ApiRoute()
                {
                    Url = baseUrl,
                    Acao = ApiRouteAction.Get,
                    Modulo = modulo
                },

                new ApiRoute()
                {
                    Url = baseUrl,
                    Acao = ApiRouteAction.Post,
                    Modulo = modulo
                },

                new ApiRoute()
                {
                    Url= baseUrl,
                    Acao = ApiRouteAction.Put,
                    Modulo = modulo
                },

                new ApiRoute()
                {
                    Url = baseUrl,
                    Acao = ApiRouteAction.Delete,
                    Modulo = modulo
                }
            };

            return rotasPadronizadas;
        }
    }
}