using Atron.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Caching;
using Shared.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atron.Tracker.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SessaoController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;
        private readonly IServiceAccessor _serviceAccessor;

        public SessaoController(ICacheService cacheService, IPerfilDeAcessoService perfilDeAcessoService, IServiceAccessor serviceAccessor)
        {
            _cacheService = cacheService;
            _perfilDeAcessoService = perfilDeAcessoService;
            _serviceAccessor = serviceAccessor;
        }

        [HttpGet("Info")]
        public async Task<ActionResult> SesssaoInfoAsync()
        {
            var user = HttpContext.User;
            var usuarioCodigo = user.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;

            if (usuarioCodigo.IsNullOrEmpty())
            {
                var codigoUsuarioRequest = Request.Headers.ExtrairCodigoUsuarioDoRequest();
                if (!codigoUsuarioRequest.IsNullOrEmpty())
                {
                    usuarioCodigo = codigoUsuarioRequest;
                }

                return NoContent();
            }

            // Tenta obter do cache
            var dadosCache = _cacheService.ObterCache<DadosComplementaresDoUsuarioDTO>(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, usuarioCodigo).KeyDescription);

            var jsonDeRetorno = new
            {
                codigoDoUsuario = usuarioCodigo,
                nomeDoUsuario = dadosCache?.DadosDoUsuario.NomeDoUsuario ?? user.FindFirst(ClaimTypes.Name)?.Value,
                emailDoUsuario = dadosCache?.DadosDoUsuario.Email ?? user.FindFirst(ClaimTypes.Email)?.Value,
                codigoDoCargo = dadosCache?.DadosDoUsuario.CodigoDoCargo ?? user.FindFirst(ClaimCode.CODIGO_CARGO)?.Value,
                codigoDoDepartamento = dadosCache?.DadosDoUsuario.CodigoDoDepartamento ?? user.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value,

                perfisDeAcesso = dadosCache?.DadosDoPerfil ??
                    _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioCodigo).Result.Select(p => new DadosDoPerfilDTO
                    {
                        CodigoPerfil = p.Codigo,
                        Modulos = p.Modulos.Select(x => new DadosDoModuloDTO(x.Codigo, x.Descricao)).ToList()
                    }).ToList()
            };

            if (dadosCache is not null)
            {
                // Retorna apenas o que o Front precisa
                return Ok(jsonDeRetorno);
            }

            // Se não havia cache (talvez expirado), busca os dados novamente
            var perfisModulos = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioCodigo);
            var dto = new DadosComplementaresDoUsuarioDTO
            {
                DadosDoUsuario = new DadosDoUsuarioDTO() { CodigoDoUsuario = usuarioCodigo, NomeDoUsuario = user.FindFirst(ClaimTypes.Name)?.Value },
                DadosDoPerfil = perfisModulos.Select(p => new DadosDoPerfilDTO
                {
                    CodigoPerfil = p.Codigo,
                    Modulos = p.Modulos.Select(x => new DadosDoModuloDTO(x.Codigo, x.Descricao)).ToList()
                }).ToList(),
            };

            // Regrava cache para próxima vez            
            _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, usuarioCodigo));

            return Ok(jsonDeRetorno);
        }
    }
}
