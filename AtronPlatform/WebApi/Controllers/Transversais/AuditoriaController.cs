using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOS.Common;
using AtronAuditoria.Application.Resources;
using AtronAuditoria.Application.UseCases;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronPlatform.WebApi.Controllers.Transversais
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly ObterAuditoriaCase _obterAuditoriaCase;

        public AuditoriaController(ObterAuditoriaCase obterAuditoriaCase)
        {
            _obterAuditoriaCase = obterAuditoriaCase;
        }

        [HttpGet("{codigoRegistro}/{contexto}")]
        public async Task<ActionResult<Resultado<AuditoriaResult>>> Get(
            string codigoRegistro,
            string contexto)
        {
            if (codigoRegistro.IsNullOrEmpty() || contexto.IsNullOrEmpty())
            {
                return BadRequest(
                    AuditoriaResource.ErroCodigoOuContextoObrigatorio);
            }

            var resultado = await _obterAuditoriaCase.ExecutarAsync(contexto, codigoRegistro);
            return Ok(resultado.Dados);
        }
    }
}