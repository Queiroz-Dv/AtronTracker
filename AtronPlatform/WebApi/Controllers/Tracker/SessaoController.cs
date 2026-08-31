using Application.Interfaces.Services;
using Application.UseCases.WorkspaceCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Users;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AtronPlatform.WebApi.Controllers.Tracker
{
    /// <summary>
    /// Controller de informações da sessão do usuário autenticado.
    /// Retorna dados complementares do usuário e seus perfis de acesso a partir do token JWT ou do cache.
    /// Requer autorização (Bearer token).
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SessaoController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IPerfilDeAcessoService _perfilDeAcessoService;        
        private readonly ObterWorkspaceAtualCase _obterWorkspaceAtualCase;

        public SessaoController(
            ICacheService cacheService,
            IPerfilDeAcessoService perfilDeAcessoService,
            IAccessorService serviceAccessor,
            ObterWorkspaceAtualCase obterWorkspaceAtualCase)
        {
            _cacheService = cacheService;
            _perfilDeAcessoService = perfilDeAcessoService;
            _obterWorkspaceAtualCase = obterWorkspaceAtualCase;
        }

        /// <summary>
        /// Retorna as informações da sessão atual do usuário autenticado.
        /// Inclui código, nome, e-mail, cargo, departamento, workspace atual e perfis de acesso com seus módulos.
        /// Utiliza cache para melhorar performance; popula o cache caso não exista entrada.
        /// </summary>
        /// <returns>200 OK com os dados da sessão, 204 NoContent se o usuário não puder ser identificado.</returns>
        [HttpGet("Info")]
        public async Task<ActionResult> SesssaoInfoAsync()
        {
            var user = HttpContext.User;
            var usuarioCodigo = user.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value;

            if (string.IsNullOrWhiteSpace(usuarioCodigo))
                return NoContent();

            var workspaceAtual = await _obterWorkspaceAtualCase.ExecutarAsync(usuarioCodigo);

            var dadosCache = _cacheService.ObterCache<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, usuarioCodigo));

            if (dadosCache is not null)
            {
                return Ok(new
                {
                    codigoDoUsuario = usuarioCodigo,
                    nomeDoUsuario = dadosCache.DadosDoUsuario.NomeDoUsuario ?? user.FindFirst(ClaimTypes.Name)?.Value,
                    emailDoUsuario = dadosCache.DadosDoUsuario.Email ?? user.FindFirst(ClaimTypes.Email)?.Value,
                    codigoDoCargo = dadosCache.DadosDoUsuario.CodigoDoCargo ?? user.FindFirst(ClaimCode.CODIGO_CARGO)?.Value,
                    codigoDoDepartamento = dadosCache.DadosDoUsuario.CodigoDoDepartamento ?? user.FindFirst(ClaimCode.CODIGO_DEPARTAMENTO)?.Value,
                    perfisDeAcesso = dadosCache.DadosDoPerfil,
                    workspaceAtual
                });
            }

            var perfisModulos = await _perfilDeAcessoService.ObterPerfisPorCodigoUsuarioAsync(usuarioCodigo);
            var dto = new DadosComplementaresDoUsuarioDTO
            {
                DadosDoUsuario = new DadosDoUsuarioDTO() { CodigoDoUsuario = usuarioCodigo, NomeDoUsuario = user.FindFirst(ClaimTypes.Name)?.Value },
                DadosDoPerfil = perfisModulos.Select(p => new DadosDoPerfilDTO
                {
                    CodigoPerfil = p.Codigo,
                    Modulos = p.Modulos.Select(x => new DadosDoModuloDTO(x.Codigo, x.Descricao)).ToList()
                }).ToList(),
            };

            _cacheService.GravarCache(new CacheInfo<DadosComplementaresDoUsuarioDTO>(new ChaveCache(ECacheKeysInfo.Acesso, usuarioCodigo))
            {
                EntityInfo = dto
            });

            var jsonAtualizado = new
            {
                codigoDoUsuario = usuarioCodigo,
                nomeDoUsuario = dto.DadosDoUsuario.NomeDoUsuario,
                emailDoUsuario = dto.DadosDoUsuario.Email,
                codigoDoCargo = dto.DadosDoUsuario.CodigoDoCargo,
                codigoDoDepartamento = dto.DadosDoUsuario.CodigoDoDepartamento,
                perfisDeAcesso = dto.DadosDoPerfil,
                workspaceAtual
            };

            return Ok(jsonAtualizado);
        }
    }
}
