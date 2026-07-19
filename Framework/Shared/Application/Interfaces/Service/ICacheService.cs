using Shared.Domain.ValueObjects;

namespace Shared.Application.Interfaces.Service
{
    public interface ICacheService
    {
        void GravarCache<T>(CacheInfo<T> cacheInfo);
        void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao);

        T ObterCache<T>(ChaveCache chaveCache);

        void RemoverCache(ChaveCache chaveCache);
    }
}
