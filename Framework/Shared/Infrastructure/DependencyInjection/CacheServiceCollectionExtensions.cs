using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Caching;
using Shared.Infrastructure.Caching;

namespace Shared.Infrastructure.DependencyInjection
{
    public static class CacheServiceCollectionExtensions
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
            else if (string.Equals(provider, "Redis", StringComparison.OrdinalIgnoreCase))
            {
                var connectionString = configuration["Cache:Redis:ConnectionString"];
                if (string.IsNullOrWhiteSpace(connectionString))
                    throw new InvalidOperationException(
                        "A conexão do Redis não foi configurada.");

                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = connectionString;
                    options.InstanceName =
                        configuration["Cache:Redis:InstanceName"]
                        ?? "atron:";
                });

                services.AddScoped<ICacheService, RedisCacheService>();
            }
            else
            {
                throw new InvalidOperationException($"Provider de cache '{provider}' nao suportado. Providers suportados: Memory, JsonFile, Redis.");
            }

            services.AddScoped<ICacheProviderInfoService, CacheProviderInfoService>();

            return services;
        }
    }
}
