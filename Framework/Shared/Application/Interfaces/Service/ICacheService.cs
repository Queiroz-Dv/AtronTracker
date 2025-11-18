using Shared.Models;

namespace Shared.Application.Interfaces.Service
{
    public interface ICacheService
    {
        void GravarCache<T>(CacheInfo<T> cacheInfo);

        T ObterCache<T>(string cacheKey);

        void RemoverCache(ECacheKeysInfo chave);
        void RemoverCache(ECacheKeysInfo chave, string codigoDaEntidade);
    }
}