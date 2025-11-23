using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities;
using Shared.Domain.ValueObjects;

namespace AtronAuditoria.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuditoriaController : ControllerBase
    {
        private readonly IAuditoriaService _service;        

        public AuditoriaController(IAuditoriaService service)
        {
            _service = service;            
        }

        [HttpGet("{codigoRegistro}")]
        public async Task<ActionResult<Resultado<Auditoria>>> Get(string codigoRegistro)
        {
            var auditoriaResultado = await _service.ObterPorCodigoRegistro(codigoRegistro);            
            return Ok(auditoriaResultado.Dado);
        }
    }    
}
