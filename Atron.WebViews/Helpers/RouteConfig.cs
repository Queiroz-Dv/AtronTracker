using Atron.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using System;

namespace Atron.WebViews.Helpers
{
    /// <summary>
    /// Classe que configura as rotas padrões para cada módulo
    /// </summary>
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
             name: "usuario",
             pattern: "Usuario/Index/{filter?}/{itemPage?}",
             defaults: new { controller = nameof(Usuario), action = nameof(Index) });

            endpoints.MapControllerRoute(
             name: "usuario",
             pattern: "Usuario/Cadastrar/{filter?}/{itemPage?}",
             defaults: new { controller = nameof(Usuario), action = "Cadastrar" });
        }
    }
}