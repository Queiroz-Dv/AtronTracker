using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;

namespace AtronTracker.Infrastructure.Authorization
{
    public class ModuloHandler : AuthorizationHandler<ModuloRequirement>
    {
        private readonly ICacheService _cacheService;
        private readonly IUsuarioService _usuarioService;
        private readonly IAccessorService _serviceAccessor;

        public ModuloHandler(
           ICacheService cacheService,
           IUsuarioService usuarioService,
           IAccessorService serviceAccessor)
        {
            _cacheService = cacheService;
            _usuarioService = usuarioService;
            _serviceAccessor = serviceAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ModuloRequirement requirement)
        {
            var usuarioCodigo = context.User.Claims.FirstOrDefault(usr => usr.Type == ClaimCode.CODIGO_USUARIO)?.Value;

            if (string.IsNullOrWhiteSpace(usuarioCodigo))
            {
                return;
            }

            var dados = _cacheService.ObterCache<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, usuarioCodigo))
            ?? await RecarregarSessaoNoCacheAsync(usuarioCodigo);

            var perfis = dados?.DadosDoPerfil;
            var modulosDosPerfis = perfis?.SelectMany(p => p.Modulos).ToList();

            if (modulosDosPerfis is null || modulosDosPerfis.Count == 0)
            {
                return;
            }

            if (modulosDosPerfis.Any(md => md.Codigo == requirement.Codigo))
            {
                context.Succeed(requirement);
            }
            else
            {
                return;
            }
        }

        private async Task<DadosComplementaresDoUsuarioDTO> RecarregarSessaoNoCacheAsync(string userId)
        {
            var dadosComplementaresService = _serviceAccessor.ObterService<IDadosComplementaresDoUsuarioService>();
            var userDto = await _usuarioService.ObterPorCodigoAsync(userId);
            var dadosDto = await dadosComplementaresService.ObterInformacoesComplementaresDoUsuario(userDto.Dados);
            _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, userId))
            {
                EntityInfo = dadosDto
            });
            return dadosDto;
        }
    }
}
