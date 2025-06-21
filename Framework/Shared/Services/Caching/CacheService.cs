using Microsoft.Extensions.Caching.Memory;
using Shared.Extensions;
using Shared.Interfaces.Caching;
using Shared.Models;

namespace Shared.Services.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public void GravarCache<T>(CacheInfo<T> cacheInfo)
        {
            _memoryCache.Set(cacheInfo.KeyDescription, cacheInfo.EntityInfo, TimeSpan.FromMinutes(30));
        }

        public T ObterCache<T>(string cacheKey)
        {
            try
            {
                var data = _memoryCache.TryGetValue(cacheKey, out T valor) ? valor : default;
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void RemoverCache(ECacheKeysInfo chave)
        {
            if (_memoryCache.TryGetValue(chave.GetDescription(), out _))
            {
                _memoryCache.Remove(chave);
            }
        }
    }
}