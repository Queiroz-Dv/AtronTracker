using Atron.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces.Caching;
using Shared.Interfaces.Services;
using Shared.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SessaoController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly ICookieService cookieService;
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;

        public SessaoController(ICacheService cacheService, IPerfilDeAcessoService perfilDeAcessoService, ICookieService cookieService)
        {
            _cacheService = cacheService;
            _perfilDeAcessoService = perfilDeAcessoService;
            this.cookieService = cookieService;
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
            var perfisDeAcesso = dadosCache?.DadosDoPerfil;

            // Se não encontrou no cache, busca de forma assíncrona
            if (perfisDeAcesso is null)
            {
                var perfis = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioServiceAsync(usuarioCodigo);
                perfisDeAcesso = perfis.Select(p => new DadosDoPerfilDTO
                {
                    CodigoPerfil = p.Codigo,
                    Modulos = p.Modulos.Select(x => new DadosDoModuloDTO(x.Codigo, x.Descricao)).ToList()
                }).ToList();
            }

            var jsonDeRetorno = new
            {
                codigoDoUsuario = usuarioCodigo,
                nomeDoUsuario = dadosCache?.DadosDoUsuario.NomeDoUsuario ?? user.FindFirst(ClaimTypes.Name)?.Value,
                emailDoUsuario = dadosCache?.DadosDoUsuario.Email ?? user.FindFirst(ClaimTypes.Email)?.Value,
                codigoDoCargo = dadosCache?.DadosDoUsuario.CodigoDoCargo ?? user.FindFirst(ClaimCode.CODIGO_CARGO)?.Value,
                codigoDoDepartamento = dadosCache?.DadosDoUsuario.CodigoDoDepartamento ?? user.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value,
                perfisDeAcesso
            };

            // Se não havia cache (talvez expirado), busca os dados novamente e regrava
            if (dadosCache is null)
            {
                // Nós já buscamos os dados, agora vamos montar o DTO para o cache
                var dtoParaCache = new DadosComplementaresDoUsuarioDTO
                {
                    // Certifique-se de preencher todos os dados necessários aqui
                    DadosDoUsuario = new DadosDoUsuarioDTO()
                    {
                        CodigoDoUsuario = usuarioCodigo,
                        NomeDoUsuario = user.FindFirst(ClaimTypes.Name)?.Value,
                        Email = user.FindFirst(ClaimTypes.Email)?.Value,
                        //... outros campos
                    },
                    DadosDoPerfil = perfisDeAcesso
                };

                // Regrava cache para próxima vez
                _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(ECacheKeysInfo.Acesso, usuarioCodigo) { EntityInfo = dadosCache }); // Passando o DTO para gravar
            }


            return Ok(jsonDeRetorno);
        }
    }
}
