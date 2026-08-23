using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using StackExchange.Redis;
using System.Text.Json;

namespace Shared.Infrastructure.Caching
{
    public sealed class RedisCacheService(
        IDistributedCache cache,
        IHostEnvironment hostEnvironment,
        ILogger<RedisCacheService> logger) : ICacheService
    {
        public void GravarCache<T>(CacheInfo<T> cacheInfo)
            => GravarCache(cacheInfo, ObterExpiracao(cacheInfo));

        public void RemoverCache(ChaveCache chaveCache)
            => ExecutarComTratativa(
                () => cache.Remove(chaveCache.Descricao),
                "remover uma entrada");

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
        {
            var json = JsonSerializer.Serialize(cacheInfo.EntityInfo);

            ExecutarComTratativa(
                () => cache.SetString(
                    cacheInfo.KeyDescription,
                    json,
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiracao }),
                "gravar uma entrada");
        }

        public T ObterCache<T>(ChaveCache chaveCache)
        {
            var json = ExecutarComTratativa(
                () => cache.GetString(chaveCache.Descricao),
                "consultar uma entrada");

            return string.IsNullOrWhiteSpace(json)
                ? default
                : JsonSerializer.Deserialize<T>(json);
        }

        private void ExecutarComTratativa(Action operacao, string descricaoOperacao)
        {
            try
            {
                operacao();
            }
            catch (RedisConnectionException) when (hostEnvironment.IsDevelopment())
            {
                AvisarRedisIndisponivel(descricaoOperacao);
            }
        }

        private TResult ExecutarComTratativa<TResult>(Func<TResult> operacao, string descricaoOperacao)
        {
            try
            {
                return operacao();
            }
            catch (RedisConnectionException) when (hostEnvironment.IsDevelopment())
            {
                AvisarRedisIndisponivel(descricaoOperacao);
                return default;
            }
        }

        private void AvisarRedisIndisponivel(string descricaoOperacao)
        {
            logger.LogWarning(
                "Redis indisponível ao {Operacao}. Inicie o Redis local com " +
                "'docker compose -f compose.redis.yaml up -d'.",
                descricaoOperacao);
        }

        private static TimeSpan ObterExpiracao<T>(CacheInfo<T> cacheInfo)
        {
            if (cacheInfo.ExpireTime == default)
                return TimeSpan.FromMinutes(30);

            var expireTime = cacheInfo.ExpireTime.Value;
            var expireTimeUtc = expireTime.Kind == DateTimeKind.Local
                ? expireTime.ToUniversalTime()
                : expireTime;

            var ttl = expireTimeUtc - DateTime.UtcNow;
            return ttl > TimeSpan.Zero ? ttl : TimeSpan.FromSeconds(1);
        }
    }
}
