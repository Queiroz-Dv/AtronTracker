using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;

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
        [Transactional]
        public async Task<ActionResult> Post([FromBody] ClienteRequest request)
        {
            var resultado = await _service.CriarAsync(request);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Response);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<ClienteRequest>>> Get()
        {
            var resultado = await _service.ObterTodosClientesServiceAsync();
            
            return Ok(resultado.Dado);
        }

        [HttpGet]
        [Route("inativos")]
        public async Task<ActionResult<ClienteRequest>> ClientesInativos()
        {
            var resultado = await _service.ObterTodosClientesInativoServiceAsync();

            return Ok(resultado.Dado);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ClienteRequest>> Get(string codigo)
        {
            var resultado = await _service.ObterClientePorCodigoServiceAsync(codigo);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Dado);
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] ClienteRequest request)
        {
            if (codigo != request.Codigo)
            {
                return BadRequest(Resultado.Falha("O código na URL não corresponde ao código no corpo da requisição.").ObterNotificacoes());
            }

            var resultado = await _service.AtualizarClienteServiceAsync(request);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Response);
        }

        [HttpPut("ativar-inativar/{codigo}/{ativar}")]
        public async Task<ActionResult> AtivarInativarCliente([FromRoute] string codigo, [FromRoute] bool ativar)
        {
            var resultado = await _service.AtivarInativarClienteServiceAsync(codigo, ativar);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Response);
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await _service.RemoverAsync(codigo);

            return resultado.TeveFalha ? BadRequest(resultado.ObterNotificacoes()) : Ok(resultado.Response);
        }
    }
}
