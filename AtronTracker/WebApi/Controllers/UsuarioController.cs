using Application.DTO.Request;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controlador para gerenciar entidades de Usuário.
    /// </summary>
    [Authorize(Policy = "Modulo:USR")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController(IUsuarioService usuarioService) : ControllerBase
    {
        /// <summary>
        /// Cria um novo usuário.
        /// </summary>
        /// <param name="request">Dados do usuário a ser criado.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UsuarioRequest request)
        {
            var resultado = await usuarioService.CriarAsync(request);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém todos os usuários.
        /// </summary>
        /// <returns>Lista de usuários.</returns>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var resultado = await usuarioService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        /// <param name="codigo">Código do usuário a ser atualizado.</param>
        /// <param name="request">Dados atualizados do usuário.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] UsuarioRequest request)
        {
            if (codigo != request.Codigo)
                return BadRequest(Resultado<object>.Falha("O código na URL não corresponde ao código no corpo da requisição.").Messages);

            var resultado = await usuarioService.AtualizarAsync(request);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Remove um usuário existente.
        /// </summary>
        /// <param name="codigo">Código do usuário a ser removido.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await usuarioService.RemoverAsync(codigo);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>
        /// Obtém um usuário pelo código.
        /// </summary>
        /// <param name="codigo">Código do usuário.</param>
        /// <returns>Dados do usuário.</returns>
        [HttpGet("{codigo}")]        
        public async Task<ActionResult> Get(string codigo)
        {
            var resultado = await usuarioService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }
    }
}