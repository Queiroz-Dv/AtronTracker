using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class DepartamentoController : ControllerBase
    {
        private readonly IDepartamentoService _departamentoService;

        public DepartamentoController(IDepartamentoService departamentoService)
        {
            // Injeta a dependência do serviço de departamento no construtor
            _departamentoService = departamentoService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DepartamentoDTO departamento)
        {            
            await _departamentoService.CriarAsync(departamento);

            return _departamentoService.GetMessages().HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()
        {
            var departamentos = await _departamentoService.ObterTodosAsync();
            return Ok(departamentos);
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            await _departamentoService.AtualizarAsync(codigo, departamento);

            return _departamentoService.GetMessages().HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Delete(string codigo)
        {
            await _departamentoService.RemoverAsync(codigo);

            return _departamentoService.GetMessages().HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var departamento = await _departamentoService.ObterPorCodigo(codigo);

            return departamento is null ?  
                NotFound(ObterNotificacoes()) :  
                Ok(departamento);            
        }

        private IEnumerable<dynamic> ObterNotificacoes()
        {
            return _departamentoService.GetMessages().ConvertMessageToJson();
        }
    }
}