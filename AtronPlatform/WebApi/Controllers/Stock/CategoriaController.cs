using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using Shared.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Stock
{
    [Authorize(Policy = ModuloPolicies.Categoria)]
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriaController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpPost]
        [Transactional]
        public async Task<ActionResult> Post([FromBody] CategoriaRequest dto)
        {
            var resultado = await _service.CriarAsync(dto);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPut("{codigo}")]
        [Transactional]
        public async Task<ActionResult> Put(string codigo, [FromBody] CategoriaRequest dto)
        {
            if (codigo != dto.Codigo)
            {
                return BadRequest(Resultado.Falha(NotificacoesPadronizadas.ErroCodigoRotaDivergente));
            }

            var resultado = await _service.AtualizarAsync(dto);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpPut("ativar-inativar/{codigo}/{ativar}")]
        [Transactional]
        public async Task<ActionResult> AtivarInativar([FromRoute] string codigo, [FromRoute] bool ativar)
        {
            var resultado = await _service.AtivarInativarAsync(codigo, ativar);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Messages);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<CategoriaRequest>>> Get()
        {
            var resultado = await _service.ObterTodasAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("inativas")]
        public async Task<ActionResult<ICollection<CategoriaRequest>>> GetInativas()
        {
            var resultado = await _service.ObterInativasAsync();
            return Ok(resultado.Dados);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<CategoriaRequest>> Get(string codigo)
        {
            var resultado = await _service.ObterPorCodigoAsync(codigo);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
