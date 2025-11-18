using Application.Interfaces.Services;
using Shared.Application.Interfaces.Service;
using Shared.DTO.API;
using Shared.Models;

namespace Application.Services.EntitiesServices
{
    public class CacheUsuarioService : ICacheUsuarioService
    {
        public ICacheService cacheService;

        public CacheUsuarioService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public void GravarCacheDeAcessoTokenInfo(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DadosDeTokenComRefreshToken tokenComRefreshToken)
        {
            var codigoUsuario = dadosDoUsuario.DadosDoUsuario.CodigoDoUsuario;

            cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, codigoUsuario)
            {
                EntityInfo = dadosDoUsuario,
                ExpireTime = tokenComRefreshToken.TokenDTO.Expires
            });

            cacheService.GravarCache(new CacheInfo<DadosDeTokenComRefreshToken>(ECacheKeysInfo.TokenInfo, codigoUsuario)
            {
                EntityInfo = tokenComRefreshToken,
                ExpireTime = tokenComRefreshToken.TokenDTO.Expires
            });
        }

        public DadosDeTokenComRefreshToken ObterDadosDoTokenPorCodigoUsuario(string codigoUsuario)
        {
            var cacheInfo = new CacheInfo<DadosDeTokenComRefreshToken>(ECacheKeysInfo.TokenInfo, codigoUsuario);
            var tokenCache = cacheService.ObterCache<DadosDeTokenComRefreshToken>(cacheInfo.KeyDescription);

            return tokenCache is null ? null : tokenCache;
        }
    }
}
