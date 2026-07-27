using AtronTracker.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AtronTracker.Infrastructure;

internal static class TrackerAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddTrackerAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddSingleton<IAuthorizationPolicyProvider, DynamicModuloPolicyProvider>();
        services.AddScoped<IAuthorizationHandler, ModuloHandler>();
        return services;
    }
}
