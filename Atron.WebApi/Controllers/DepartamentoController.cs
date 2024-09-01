using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Notification.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
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

            var messages = _departamentoService.Messages.ConvertMessageToJson();
            return Ok(messages);
        }

        [HttpGet]
        [Route("ObterDepartamentos")]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()
        {
            var departamentos = await _departamentoService.ObterTodosAsync();
            if (departamentos is null)
            {
                return NotFound("Não foi encontrado nenhum registro");
            }

            return Ok(departamentos);
        }

        [HttpPut("AtualizarDepartamento/{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            if (codigo != departamento.Codigo || codigo is null)
            {
                return BadRequest();
            }

            await _departamentoService.AtualizarAsync(departamento);
            if (_departamentoService.notificationMessages.HasErrors())
            {
                return BadRequest(_departamentoService.notificationMessages);
            }

            return Ok(_departamentoService.notificationMessages);
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

            return Ok(_departamentoService.notificationMessages);
        }
    }
}