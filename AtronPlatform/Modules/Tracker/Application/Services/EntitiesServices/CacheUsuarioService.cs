using Application.Interfaces.Services;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System;

namespace Application.Services.EntitiesServices
{
    public class CacheUsuarioService : ICacheUsuarioService
    {
        public ICacheService cacheService;

        public CacheUsuarioService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public void GravarCacheDeAcesso(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DateTime expiracaoAccessToken)
        {
            var codigoUsuario = dadosDoUsuario.DadosDoUsuario.CodigoDoUsuario;

            cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, codigoUsuario))
            {
                EntityInfo = dadosDoUsuario,
                ExpireTime = expiracaoAccessToken
            });
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
