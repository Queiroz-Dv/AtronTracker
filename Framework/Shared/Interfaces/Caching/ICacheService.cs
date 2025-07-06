using Shared.Models;

namespace Shared.Interfaces.Caching
{
    public interface ICacheService
    {
        void GravarCache<T>(CacheInfo<T> cacheInfo);
               
        T ObterCache<T>(string cacheKey);

        void RemoverCache(ECacheKeysInfo chave);
        void RemoverCache(ECacheKeysInfo chave, string codigoDaEntidade);
    }
}