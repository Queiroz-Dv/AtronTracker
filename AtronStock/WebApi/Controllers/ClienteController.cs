using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace AtronStock.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClienteController(IClienteService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ClienteRequest request)
        {
            var resultado = await _service.CriarAsync(request);

            if (resultado.TeveFalha)
            {
                return BadRequest(resultado.ObterNotificacoes());
            }

            return Ok(resultado.Response);
        }
        
        [HttpGet]
        public async Task<ActionResult<ICollection<ClienteRequest>>> Get()
        {            
            var resultado = await _service.ObterTodosClientesServiceAsync();

            if (resultado.TeveFalha)
            {
                return BadRequest(resultado.ObterNotificacoes());
            }
         
            return Ok(resultado.Dado);
        }
        
        [HttpGet("{codigo}")]
        public async Task<ActionResult<ClienteRequest>> Get(string codigo)
        {
            var resultado = await _service.ObterClientePorCodigoServiceAsync(codigo);

            if (resultado.TeveFalha)
            {                
                if (resultado.Response.Any(m => m.Descricao.Contains("não existe")))
                {
                    return NotFound(resultado.ObterNotificacoes());
                }

                return BadRequest(resultado.ObterNotificacoes());
            }
            
            return Ok(resultado.Dado);
        }

 
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] ClienteRequest request)
        {
            if (codigo != request.Codigo)
            {
                return BadRequest(Resultado.Falha("O código na URL não corresponde ao código no corpo da requisição.").ObterNotificacoes());
            }

            var resultado = await _service.AtualizarClienteServiceAsync(request);

            if (resultado.TeveFalha)
            {                
                if (resultado.Response.Any(m => m.Descricao.Contains("não existe")))
                {
                    return NotFound(resultado.ObterNotificacoes());
                }

                return BadRequest(resultado.ObterNotificacoes());
            }
            
            return Ok(resultado.Response);
        }
        
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await _service.RemoverAsync(codigo);

            if (resultado.TeveFalha)
            {              
                if (resultado.Response.Any(m => m.Descricao.Contains("não encontrado")))
                {
                    return NotFound(resultado.ObterNotificacoes());
                }

                return BadRequest(resultado.ObterNotificacoes());
            }
           
            return Ok(resultado.Response);
        }
    }
}
