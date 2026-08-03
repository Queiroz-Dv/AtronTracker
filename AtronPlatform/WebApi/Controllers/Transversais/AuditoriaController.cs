using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronPlatform.WebApi.Controllers.Transversais
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _service;

        public AuditoriaController(IAuditoriaService service)
        {
            _service = service;
        }

        [HttpGet("{codigoRegistro}/{contexto}")]
        public async Task<ActionResult<Resultado<Auditoria>>> Get(
            string codigoRegistro,
            string contexto)
        {
            if (codigoRegistro.IsNullOrEmpty() || contexto.IsNullOrEmpty())
            {
                return BadRequest(
                    AuditoriaResource.ErroCodigoOuContextoObrigatorio);
            }

            IAuditoriaDTO auditoria = new AuditoriaDTO
            {
                CodigoRegistro = codigoRegistro,
                Contexto = contexto
            };

            var resultado = await _service.ObterPorChaveServiceAsync(auditoria);
            return Ok(resultado.Dados);
        }
    }
}
