using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>  
    /// Controlador para gerenciar entidades de Cargo.  
    /// </summary>       
    [Authorize(Policy = "Modulo:CRG")]
    [ApiController]
    [Route("api/[controller]")]
    public class CargoController(ICargoService cargoService) : ControllerBase
    {
        /// <summary>  
        /// Cria um novo cargo.  
        /// </summary>  
        /// <param name="cargo">Dados do cargo a ser criado.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CargoDTO cargo)
        {
            var resultado = await cargoService.CriarAsync(cargo);

            return resultado.TeveFalha ?
                BadRequest(resultado.ObterNotificacoes()) :
                Ok(resultado.Response);
        }

        /// <summary>  
        /// Obtém todos os cargos.  
        /// </summary>  
        /// <returns>Lista de cargos.</returns>  
        [HttpGet]
        public async Task<ActionResult<ICollection<CargoDTO>>> Get()
        {
            var resultado = await cargoService.ObterTodosAsync();
            return Ok(resultado.Entidade);
        }

        /// <summary>  
        /// Atualiza um cargo existente.  
        /// </summary>  
        /// <param name="codigo">Código do cargo a ser atualizado.</param>  
        /// <param name="cargo">Dados atualizados do cargo.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] CargoDTO cargo)
        {
            if (codigo != cargo.Codigo)
                return BadRequest(Resultado.Falha("O código na URL não corresponde ao código no corpo da requisição.").ObterNotificacoes());

            var resultado = await cargoService.AtualizarAsync(codigo, cargo);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Response);
        }

        /// <summary>  
        /// Remove um cargo existente.  
        /// </summary>  
        /// <param name="codigo">Código do cargo a ser removido.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await cargoService.RemoverAsync(codigo);

            return resultado.TeveFalha ?
                BadRequest(resultado.ObterNotificacoes()) :
                Ok(resultado.Response);
        }

        /// <summary>  
        /// Obtém um cargo pelo código.  
        /// </summary>  
        /// <param name="codigo">Código do cargo.</param>  
        /// <returns>Dados do cargo.</returns>  
        [HttpGet("{codigo}")]
        public async Task<ActionResult<CargoDTO>> Get(string codigo)
        {
            var resultado = await cargoService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha ?
                NotFound(resultado.ObterNotificacoes()) :
                Ok(resultado.Entidade);
        }
    }
}