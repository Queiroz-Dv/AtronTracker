using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>  
    /// Controlador para gerenciar entidades de Departamento.  
    /// </summary>       
    [Authorize(Policy = "Modulo:DPT")]
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentoController(IDepartamentoService departamentoService) : ControllerBase
    {
        /// <summary>  
        /// Cria um novo departamento.  
        /// </summary>  
        /// <param name="departamento">Dados do departamento a ser criado.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPost]    
        public async Task<ActionResult> Post([FromBody] DepartamentoDTO departamento)
        {
            var resultado = await departamentoService.CriarAsync(departamento);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>  
        /// Obtém todos os departamentos.  
        /// </summary>  
        /// <returns>Lista de departamentos.</returns>  
        [HttpGet]
        public async Task<ActionResult<ICollection<DepartamentoDTO>>> Get()
        {
            var resultado = await departamentoService.ObterTodosAsync();
            return Ok(resultado.Dados);
        }

        /// <summary>  
        /// Atualiza um departamento existente.  
        /// </summary>  
        /// <param name="codigo">Código do departamento a ser atualizado.</param>  
        /// <param name="departamento">Dados atualizados do departamento.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            if (codigo != departamento.Codigo)
                return BadRequest(Resultado<object>.Falha("O código na URL não corresponde ao código no corpo da requisição.").Messages);

            var resultado = await departamentoService.AtualizarAsync(codigo, departamento);

            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        /// <summary>  
        /// Remove um departamento existente.  
        /// </summary>  
        /// <param name="codigo">Código do departamento a ser removido.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await departamentoService.RemoverAsync(codigo);

            return resultado.TeveFalha ?
                BadRequest(resultado.Messages) :
                Ok(resultado.Messages);
        }

        /// <summary>  
        /// Obtém um departamento pelo código.  
        /// </summary>  
        /// <param name="codigo">Código do departamento.</param>  
        /// <returns>Dados do departamento.</returns>  
        [HttpGet("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var resultado = await departamentoService.ObterPorCodigo(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.Messages) :
                Ok(resultado.Dados);
        }
    }
}