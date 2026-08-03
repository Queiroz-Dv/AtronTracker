using Microsoft.Extensions.Caching.Memory;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Shared.Application.Services.Caching
{
    public class CacheService(IMemoryCache memoryCache) : ICacheService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;

        public void GravarCache<T>(CacheInfo<T> cacheInfo)
        {
            _memoryCache.Set(
                cacheInfo.KeyDescription,
                cacheInfo.EntityInfo,
                ObterExpiracaoRelativa(cacheInfo));
        }

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
        {
            _memoryCache.Set(
                cacheInfo.KeyDescription,
                cacheInfo.EntityInfo,
                expiracao);
        }

        public T ObterCache<T>(ChaveCache chaveCache)
        {
            try
            {
                var data = _memoryCache.TryGetValue(chaveCache.Descricao, out T valor) ? valor : default;
                return data;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void RemoverCache(ChaveCache chaveCache)
        {
            if (_memoryCache.TryGetValue(chaveCache.Descricao, out _))
            {
                _memoryCache.Remove(chaveCache.Descricao);
            }
        }

        private static TimeSpan ObterExpiracaoRelativa<T>(CacheInfo<T> cacheInfo)
        {
            if (cacheInfo.ExpireTime == default)
                return TimeSpan.FromMinutes(30);

            var expireTimeUtc = cacheInfo.ExpireTime.Kind == DateTimeKind.Local
                ? cacheInfo.ExpireTime.ToUniversalTime()
                : cacheInfo.ExpireTime;

            var ttl = expireTimeUtc - DateTime.UtcNow;
            return ttl > TimeSpan.Zero ? ttl : TimeSpan.FromSeconds(1);
        }
    }
}
