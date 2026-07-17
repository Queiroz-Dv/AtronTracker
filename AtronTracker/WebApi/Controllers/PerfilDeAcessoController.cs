using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de perfis de acesso e seus relacionamentos com usuários.
    /// Perfis de acesso agrupam módulos e são atribuídos aos usuários para controle de permissões.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilDeAcessoController(IPerfilDeAcessoService perfilDeAcessoService) : ControllerBase
    {
        /// <summary>
        /// Cria um novo perfil de acesso.
        /// </summary>
        /// <param name="perfilDeAcessoDTO">Dados do perfil de acesso a ser criado, incluindo código, descrição e módulos associados.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            var resultado = await perfilDeAcessoService.CriarAsync(perfilDeAcessoDTO);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Atualiza um perfil de acesso existente identificado pelo código.
        /// </summary>
        /// <param name="codigo">Código do perfil de acesso a ser atualizado.</param>
        /// <param name="perfilDeAcessoDTO">Dados atualizados do perfil de acesso.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPut("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Put(string codigo, [FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            if (perfilDeAcessoDTO is null)
                return BadRequest(Resultado<object>.Falha(PerfilDeAcessoResource.Erro_PerfilInvalido).Messages);

            if (codigo != perfilDeAcessoDTO.Codigo)
                return BadRequest(Resultado<object>.Falha(NotificacoesPadronizadas.ErroCodigoRotaDivergente).Messages);

            var resultado = await perfilDeAcessoService.AtualizarAsync(codigo, perfilDeAcessoDTO);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém todos os perfis de acesso cadastrados no sistema.
        /// </summary>
        /// <returns>200 OK com a coleção de perfis de acesso.</returns>
        [HttpGet]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult<ICollection<PerfilDeAcessoDTO>>> Get()
        {
            var resultado = await perfilDeAcessoService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Obtém um perfil de acesso pelo código.
        /// </summary>
        /// <param name="codigo">Código do perfil de acesso.</param>
        /// <returns>200 OK com os dados do perfil ou 404 NotFound se não encontrado.</returns>
        [HttpGet("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult<PerfilDeAcessoDTO>> Get(string codigo)
        {
            var resultado = await perfilDeAcessoService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }

        /// <summary>
        /// Remove um perfil de acesso pelo código.
        /// </summary>
        /// <param name="codigo">Código do perfil de acesso a ser removido.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpDelete("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await perfilDeAcessoService.RemoverAsync(codigo);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Cria ou atualiza o relacionamento entre um perfil de acesso e uma lista de usuários.
        /// Substitui os usuários atuais do perfil pelos informados na requisição.
        /// </summary>
        /// <param name="request">Objeto com o código do perfil e a lista de usuários a serem associados.</param>
        /// <returns>200 OK com mensagens de sucesso ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("RelacionamentoPerfilUsuario")]
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
        public async Task<ActionResult> RelacionamentoPerfilUsuario([FromBody] PerfilUsuarioRequest request)
        {
            if (request is null)
                return BadRequest(Resultado<object>.Falha(PerfilDeAcessoResource.Erro_RelacionamentoInvalido).Messages);

            var dto = new PerfilDeAcessoUsuarioDTO
            {
                PerfilDeAcesso = new PerfilDeAcessoDTO { Codigo = request.CodigoPerfil },
                Usuarios = (request.Usuarios ?? []).Select(usuario => new UsuarioDTO { Codigo = usuario.Codigo }).ToList()
            };

            var resultado = await perfilDeAcessoService.RelacionarPerfilDeAcessoUsuarioAsync(dto);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém os usuários associados a um perfil de acesso pelo código do perfil.
        /// </summary>
        /// <param name="codigo">Código do perfil de acesso cujos relacionamentos serão consultados.</param>
        /// <returns>200 OK com o DTO do relacionamento ou 404 NotFound se não encontrado.</returns>
        [HttpGet("ObterRelacionamentoPerfilUsuario/{codigo}")]
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
        public async Task<ActionResult<PerfilDeAcessoUsuarioDTO>> ObterRelacionamentosPerfilUsuario([FromRoute] string codigo)
        {
            var resultado = await perfilDeAcessoService.ObterRelacionamentoDePerfilUsuarioPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }
    }
}
