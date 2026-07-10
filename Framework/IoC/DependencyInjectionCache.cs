using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Caching;

namespace IoC
{
    public static class DependencyInjectionCache
    {
        public static IServiceCollection AddAtronCache(this IServiceCollection services, IConfiguration configuration)
        {
            var provider = configuration["Cache:Provider"];
            if (string.IsNullOrWhiteSpace(provider))
                provider = "Memory";

            if (string.Equals(provider, "Memory", StringComparison.OrdinalIgnoreCase))
            {
                services.AddMemoryCache();
                services.AddScoped<ICacheService, CacheService>();
            }
            else if (string.Equals(provider, "JsonFile", StringComparison.OrdinalIgnoreCase)
                || string.Equals(provider, "ArquivoJson", StringComparison.OrdinalIgnoreCase))
            {
                services.AddScoped<ICacheService, JsonFileCacheService>();
            }
            else
            {
                throw new InvalidOperationException($"Provider de cache '{provider}' nao suportado. Providers suportados: Memory, JsonFile.");
            }

            services.AddScoped<ICacheProviderInfoService, CacheProviderInfoService>();

            return services;
        }
    }
}
