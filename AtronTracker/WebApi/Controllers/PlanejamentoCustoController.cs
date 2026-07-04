using Application.DTO.Request;
using Application.DTO.Response;
using Application.Extensions;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Authorize(Policy = ModuloPolicies.PlanejamentoCustos)]
    [ApiController]
    [Route("api/[controller]")]
    public class PlanejamentoCustoController(IPlanejamentoCustoService planejamentoCustoService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<PlanejamentoCustoResponse>> Post([FromBody] PlanejamentoCustoRequest request)
        {
            var resultado = await planejamentoCustoService.CriarAsync(request.MontarDTO());

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<PlanejamentoCustoResponse>>> Get([FromQuery] int? ano)
        {
            var resultado = ano.HasValue
                ? await planejamentoCustoService.ObterPorAnoAsync(ano.Value)
                : await planejamentoCustoService.ObterTodosAsync();

            var response = resultado.Dados.Select(plc => plc.MontarResponse()).ToList();
            return Ok(response);
        }

        [HttpGet("relatorio-geral")]
        public async Task<ActionResult<PlanejamentoCustoRelatorioGeralResponse>> ObterRelatorioGeral([FromQuery] int ano)
        {
            var resultado = await planejamentoCustoService.ObterRelatorioGeralAsync(ano);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        [HttpGet("relatorio/{codigo}")]
        public async Task<ActionResult<PlanejamentoCustoRelatorioGeralResponse>> ObterRelatorioPorCodigo(string codigo)
        {
            var resultado = await planejamentoCustoService.ObterRelatorioPorCodigoAsync(codigo);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<PlanejamentoCustoResponse>> Get(string codigo)
        {
            var resultado = await planejamentoCustoService.ObterPorCodigoAsync(codigo);

            return resultado.TeveFalha
                ? NotFound(resultado.Messages)
                : Ok(resultado.Dados.MontarResponse());
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] PlanejamentoCustoRequest request)
        {
            var resultado = await planejamentoCustoService.AtualizarAsync(codigo, request.MontarDTO());

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var resultado = await planejamentoCustoService.RemoverAsync(codigo);

            return resultado.TeveFalha
                ? BadRequest(resultado.Messages)
                : Ok(resultado.Messages);
        }
    }
}
