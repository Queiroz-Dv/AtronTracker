using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionAuthorizationPolicy
    {
        public static IServiceCollection AddModuleAuthorizationPolicies(this IServiceCollection services, IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var moduloService = scope.ServiceProvider.GetRequiredService<IModuloService>();
                var moduleCodes = moduloService.ObterTodosOsCodigos();

                services.AddAuthorization(options =>
                {
                    //foreach (var code in moduleCodes)
                    //{
                    //    options.AddPolicy($"Modulo:{code}", new AuthorizationPolicyBuilder()
                    //        .RequireAuthenticatedUser()
                    //        .Requirements.Add(new ModuloAuthorizationRequirement(code))
                    //        .Build());
                    //}
                });
            }
            return services;
        }
    }
}
