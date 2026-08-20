using Application.Interfaces.Services;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System;

namespace Application.Services.AuthServices
{
    public class CacheUsuarioService(ICacheService cacheService) : ICacheUsuarioService
    {
        private readonly ICacheService _cacheService = cacheService;

        public void GravarCacheDeAcesso(DadosComplementaresDoUsuarioDTO dadosDoUsuario, DateTime expiracaoAccessToken)
        {
            var codigoUsuario = dadosDoUsuario.DadosDoUsuario.CodigoDoUsuario;
            var chaveCache = new ChaveCache(ECacheKeysInfo.Acesso, codigoUsuario);
            var cacheInfo = new CacheInfo<DadosComplementaresDoUsuarioDTO>(chaveCache);
            cacheInfo.VincularDadosTemporarios(dadosDoUsuario, expiracaoAccessToken);

            _cacheService.GravarCache(cacheInfo);
        }

        public void RemoverCacheDeAcessoTokenInfo(string codigoUsuario)
        {
            if (string.IsNullOrWhiteSpace(codigoUsuario))
                return;

            _cacheService.RemoverCache(new ChaveCache(ECacheKeysInfo.Acesso, codigoUsuario));
            _cacheService.RemoverCache(new ChaveCache(ECacheKeysInfo.TokenInfo, codigoUsuario));
        }
    }
}