using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Helpers
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

            if (usuarioCodigo.IsNullOrEmpty() && context.Resource is AuthorizationFilterContext mvcContext)
            {
                usuarioCodigo = mvcContext.HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            }

            if (usuarioCodigo.IsNullOrEmpty())
            {
                return;
            }

            var dados = _cacheService.ObterCache<DadosComplementaresDoUsuarioDTO>(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, usuarioCodigo).KeyDescription)
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
            _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, userId)
            {
                EntityInfo = dadosDto
            });
            return dadosDto;
        }
    }
}
