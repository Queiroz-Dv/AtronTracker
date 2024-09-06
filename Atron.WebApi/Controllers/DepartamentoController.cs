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

        [HttpPost("CriarDepartamento")]
        public async Task<ActionResult> Post([FromBody] DepartamentoDTO departamento)
        {
            if (departamento == null)
            {
                return BadRequest("Registro inválido, tente novamente.");
            }

            await _departamentoService.CriarAsync(departamento);
            var messages = ObterNotificacoes();
            return Ok(messages);
        }

        private IEnumerable<dynamic> ObterNotificacoes()
        {
            return _departamentoService.GetMessages().ConvertMessageToJson();
        }

        [HttpGet]
        [Route("ObterDepartamentos")]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()
        {
            var departamentos = await _departamentoService.ObterTodosAsync();
            return Ok(departamentos);
        }

        [HttpPut("AtualizarDepartamento/{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            if (ModelState.IsValid)
            {
                await _departamentoService.AtualizarAsync(codigo, departamento);
                if (_departamentoService.GetMessages().HasErrors())
                {
                    return BadRequest(ObterNotificacoes());
                }

                return Ok(ObterNotificacoes());
            }
            else
            {
                return BadRequest(ObterNotificacoes());
            }
        }

        [HttpDelete("ExcluirDepartamento/{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Delete(string codigo)
        {
            var departamento = await _departamentoService.ObterPorCodigo(codigo);

            if (departamento is null)
            {
                return NotFound(new NotificationMessage("Departamento não encontrado"));
            }

            await _departamentoService.RemoverAsync(codigo);

            return Ok(ObterNotificacoes());
        }

        [HttpGet("ObterPorCodigo/{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var departamento = await _departamentoService.ObterPorCodigo(codigo);

            if (departamento is null)
            {
                return NotFound(ObterNotificacoes());
            }

            return Ok(departamento);
        }
    }
}