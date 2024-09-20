using Atron.Domain.ApiEntities;

namespace Atron.Domain.Extensions
{
    public static class ApirRouteExtensions
    {
        public static string BuildUri(this ApiRoute route, string parameter = "")
        {
            if (string.IsNullOrEmpty(parameter))
            {
                return route is null ? null : $"{route.Url}/{route.Modulo}";
            }
            else
            {
                return $"{route.Url}/{route.Modulo}/{parameter}";
            }
        }
    }
}