using Shared.Application.DTOS.Common;

namespace Shared.Application.Interfaces.Service
{
    public interface ICacheProviderInfoService
    {
        CacheProviderInfoDTO ObterInfo();
    }
}
