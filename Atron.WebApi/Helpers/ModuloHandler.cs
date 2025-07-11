using Atron.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Caching;
using Shared.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebApi.Helpers
{
    /// <summary>
    /// Handler para verificar se o usuário tem acesso a um módulo específico.
    /// </summary>
    public class ModuloHandler : AuthorizationHandler<ModuloRequirement>
    {
        private readonly ICacheService _cacheService;
        private readonly IUsuarioService _usuarioService;
        private readonly IServiceAccessor _serviceAccessor;
        //private readonly IUsuarioHandler _usuarioHandler;

        public ModuloHandler(
            ICacheService cacheService,
            IUsuarioService usuarioService)
        //IUsuarioHandler usuarioHandler)
        {
            _cacheService = cacheService;
            _usuarioService = usuarioService;
            //_usuarioHandler = usuarioHandler;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ModuloRequirement requirement)
        {
            // Extract the userId from the claim  
            var usuarioCodigo = context.User.Claims.FirstOrDefault(usr => usr.Type == ClaimCode.CODIGO_USUARIO)?.Value;

            if (usuarioCodigo.IsNullOrEmpty() && context.Resource is AuthorizationFilterContext mvcContext)
            {
                usuarioCodigo = mvcContext.HttpContext.Request.Headers.ExtrairCodigoUsuarioDoRequest();
            }

            if (usuarioCodigo.IsNullOrEmpty())
            {
                // Não está autenticado — deixa o pipeline de authorization resolver (401)
                return;
            }

            // Attempt to retrieve from cache: List<PerfilComModulos>  
            var dados = _cacheService.ObterCache<DadosComplementaresDoUsuarioDTO>(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, usuarioCodigo).KeyDescription)
            ?? await RecarregarSessaoNoCacheAsync(usuarioCodigo);

            // 4. Verifique o módulo
            var perfis = dados?.DadosDoPerfil;
            var modulosDosPerfis = perfis?.SelectMany(p => p.Modulos).ToList();

            if (modulosDosPerfis is null || modulosDosPerfis.Count == 0)
            {
                // Não tem perfis ou módulos — deixa o pipeline de authorization resolver (403)
                return;
            }

            if(modulosDosPerfis.Any(md => md.Codigo == requirement.Codigo))
            {
                context.Succeed(requirement);
            }
            else
            {
                // Não tem o módulo requerido — deixa o pipeline de authorization resolver (403)
                return;
            }
        }

        // Método auxiliar
        private async Task<DadosComplementaresDoUsuarioDTO> RecarregarSessaoNoCacheAsync(string userId)
        {
            var dadosComplementaresService = _serviceAccessor.ObterService<IDadosComplementaresDoUsuarioService>();
            var userDto = await _usuarioService.ObterPorCodigoAsync(userId);
            var dadosDto = await dadosComplementaresService.ObterInformacoesComplementaresDoUsuario(userDto);            
            _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, userId));
            return dadosDto;
        }
    }
}