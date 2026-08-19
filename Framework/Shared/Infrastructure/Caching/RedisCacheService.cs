using Microsoft.Extensions.Caching.Distributed;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System.Text.Json;

namespace Shared.Infrastructure.Caching
{
    public sealed class RedisCacheService(IDistributedCache cache) : ICacheService
    {
        public void GravarCache<T>(CacheInfo<T> cacheInfo) => GravarCache(cacheInfo, ObterExpiracao(cacheInfo));
        public void RemoverCache(ChaveCache chaveCache) => cache.Remove(chaveCache.Descricao);

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
        {
            var json = JsonSerializer.Serialize(cacheInfo.EntityInfo);

            cache.SetString(cacheInfo.KeyDescription, json,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = expiracao });
        }

        public T ObterCache<T>(ChaveCache chaveCache)
        {
            var json = cache.GetString(chaveCache.Descricao);

            return string.IsNullOrWhiteSpace(json)
                ? default
                : JsonSerializer.Deserialize<T>(json);

        }


        private static TimeSpan ObterExpiracao<T>(
        CacheInfo<T> cacheInfo)
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
