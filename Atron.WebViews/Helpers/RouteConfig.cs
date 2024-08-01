using Atron.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

namespace Atron.WebViews.Helpers
{
    public static class RouteConfig
    {
        public static void AddEntityRoutes(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapControllerRoute(
                name: "departamento",
                pattern: "Departamento/Index/{filter?}/{itemPage?}",             
                defaults: new { controller = nameof(Departamento), action = nameof(Index)});

            endpoints.MapControllerRoute(
               name: "cargo",
               pattern: "Cargo/Index/{filter?}/{itemPage?}",
               defaults: new { controller = nameof(Cargo), action = nameof(Index) });

            endpoints.MapControllerRoute(
               name: "cargo",
               pattern: "Cargo/Cadastrar/{filter?}/{itemPage?}",
               defaults: new { controller = nameof(Cargo), action = "Cadastrar" });

            //endpoints.MapControllerRoute(
            //name: "cargo",
            //pattern: "Cargo/Cadastrar/{itemPage?}",
            //defaults: new { controller = nameof(Cargo), action = "Cadastrar" });
        }
    }
}