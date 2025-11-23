using Microsoft.Extensions.Caching.Memory;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

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
                TimeSpan.FromMinutes(30));
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

        public void RemoverCache(ECacheKeysInfo chave, string codigoDaEntidade)
        {
            var cacheKey = $"{chave.GetDescription()}:{codigoDaEntidade}";

            if (_memoryCache.TryGetValue(cacheKey, out _))
            {
                _memoryCache.Remove(cacheKey);
            }
        }
    }
}