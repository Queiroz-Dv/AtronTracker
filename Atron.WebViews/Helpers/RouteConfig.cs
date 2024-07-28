using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Atron.WebViews.Helpers
{
    public static class RouteConfig
    {
        public static void AddEntityRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "departamento",
                pattern: "Departamento/Index/{filter?}/{itemPage?}",
                defaults: new { controller = "Departamento", action = "Index" });
        }
    }

}
