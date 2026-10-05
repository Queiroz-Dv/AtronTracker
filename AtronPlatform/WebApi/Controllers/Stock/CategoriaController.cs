using AtronStock.Application.DTO.Request;
using AtronStock.Application.UseCases.CategoriaCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Resources;
using Shared.Authorization;
using Shared.Domain.ValueObjects;

namespace AtronPlatform.WebApi.Controllers.Stock
{
    /// <summary>  
    /// Controlador para gerenciar entidades de Categoria.  
    /// </summary>
    [Authorize(Policy = ModuloPolicies.Categoria)]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController(
        CriarCategoriaCase criarCategoria,
        AtualizarCategoriaCase atualizarCategoria,
        ExcluirCategoriaCase excluirCategoria,
        ObterCategoriaCase obterCategoria,
        AtivarInativarCategoriaCase ativarInativarCategoria) : ControllerBase
    {
        /// <summary>  
        /// Cria uma nova categoria.  
        /// </summary>  
        /// <param name="dto">Dados da categoria a ser criada.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CategoriaRequest dto)
        {
            var resultado = await criarCategoria.ExecutarAsync(dto);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>  
        /// Atualiza uma categoria existente.  
        /// </summary>  
        /// <param name="codigo">Código da categoria a ser atualizada.</param>  
        /// <param name="dto">Dados atualizados da categoria.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] CategoriaRequest dto)
        {
            if (codigo != dto.Codigo)
            {
                return BadRequest(Resultado.Falha(NotificacoesPadronizadas.ErroCodigoRotaDivergente).Messages);
            }

            var resultado = await atualizarCategoria.ExecutarAsync(dto);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>  
        /// Ativa ou inativa uma categoria.  
        /// </summary>  
        /// <param name="codigo">Código da categoria.</param>  
        /// <param name="ativar">Define se a categoria será ativada ou inativada.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPut("ativar-inativar/{codigo}/{ativar}")]
        public async Task<ActionResult> AtivarInativar([FromRoute] string codigo, [FromRoute] bool ativar)
        {
            var resultado = await ativarInativarCategoria.ExecutarAsync(codigo, ativar);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>  
        /// Remove uma categoria existente.  
        /// </summary>  
        /// <param name="codigo">Código da categoria a ser removida.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await excluirCategoria.ExecutarAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>  
        /// Obtém todas as categorias.  
        /// </summary>  
        /// <returns>Lista de categorias.</returns>  
        [HttpGet]
        public async Task<ActionResult<ICollection<CategoriaRequest>>> Get()
        {
            var resultado = await obterCategoria.ObterTodasAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>  
        /// Obtém todas as categorias inativas.  
        /// </summary>  
        /// <returns>Lista de categorias inativas.</returns>  
        [HttpGet("inativas")]
        public async Task<ActionResult<ICollection<CategoriaRequest>>> GetInativas()
        {
            var resultado = await obterCategoria.ObterInativasAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>  
        /// Obtém uma categoria pelo código.  
        /// </summary>  
        /// <param name="codigo">Código da categoria.</param>  
        /// <returns>Dados da categoria.</returns>  
        [HttpGet("{codigo}")]
        public async Task<ActionResult<CategoriaRequest>> Get(string codigo)
        {
            var resultado = await obterCategoria.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha ? NotFound(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
