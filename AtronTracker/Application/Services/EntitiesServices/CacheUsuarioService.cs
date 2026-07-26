using Application.Interfaces.Services;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

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

            cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, codigoUsuario))
            {
                EntityInfo = dadosDoUsuario,
                ExpireTime = tokenComRefreshToken.TokenDTO.Expires
            });

            cacheService.GravarCache(new CacheInfo<DadosDeTokenComRefreshToken>(new ChaveCache(ECacheKeysInfo.TokenInfo, codigoUsuario))
            {
                EntityInfo = tokenComRefreshToken,
                ExpireTime = tokenComRefreshToken.TokenDTO.Expires
            });
        }

        public DadosDeTokenComRefreshToken ObterDadosDoTokenPorCodigoUsuario(string codigoUsuario)
        {
            var chaveCache = new ChaveCache(ECacheKeysInfo.TokenInfo, codigoUsuario);
            var tokenCache = cacheService.ObterCache<DadosDeTokenComRefreshToken>(chaveCache);

            return tokenCache is null ? null : tokenCache;
        }

        public void RemoverCacheDeAcessoTokenInfo(string codigoUsuario)
        {
            if (string.IsNullOrWhiteSpace(codigoUsuario))
                return;

            cacheService.RemoverCache(new ChaveCache(ECacheKeysInfo.Acesso, codigoUsuario));
            cacheService.RemoverCache(new ChaveCache(ECacheKeysInfo.TokenInfo, codigoUsuario));
        }
    }
}
