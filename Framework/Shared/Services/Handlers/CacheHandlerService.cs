using Microsoft.AspNetCore.Http;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Handlers;
using Shared.Models;

namespace Shared.Services.Handlers
{
    public class CacheHandlerService : ICacheHandlerService
    {
        private readonly ICacheService _cacheService;

        public CacheHandlerService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public InfoToken ObterInfoTokensDoCachePorRequest(HttpRequest request)
        {
            if (request.IsRefreshing())
            {
                var cacheInfo = new CacheInfo<InfoToken>(ECacheKeysInfo.TokenInfo, request.ExtrairCodigoUsuarioDoRequest());
                var infoTokenCache = _cacheService.ObterCache<InfoToken>(cacheInfo.KeyDescription);

                if (infoTokenCache is null)
                {
                    return null;
                }

                return new InfoToken
                {
                    Token = infoTokenCache.Token,
                    InfoRefreshToken = infoTokenCache.InfoRefreshToken,
                    Expires = infoTokenCache.Expires,
                    RefreshTokenExpireTime = infoTokenCache.RefreshTokenExpireTime
                };
            }

            return null;
        }
    }
}