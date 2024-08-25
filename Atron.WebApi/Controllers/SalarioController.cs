using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalarioController : ControllerBase
    {
        private readonly ISalarioService _service;

        public SalarioController(ISalarioService service)
        {
            _service = service;
        }

        [Route("CriarSalario")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalarioDTO salario)
        {
            if (salario is null)
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));

            await _service.CriarAsync(salario);

            return Ok(_service.Messages);
        }

        [Route("ObterSalarios")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalarioDTO>>> Get()
        {
            var salarios = await _service.ObterTodosAsync();
            if (salarios is null)
                NotFound("Não foi encontrado nenum registro");

            return Ok(salarios);
        }

        [HttpPut("AtualizarSalario/{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalarioDTO salario)
        {
            await _service.AtualizarServiceAsync(salario);

            if (_service.Messages.HasErrors())
            {
                foreach (var item in _service.Messages)
                    return BadRequest(item.Message);
            }

            return Ok(_service.Messages);
        }

        // TODO: Testar depois
        [HttpDelete("ExcluirPermissao")]
        public async Task<ActionResult> Delete(int id)
        {
            await _service.ExcluirServiceAsync(id);

            if (_service.Messages.HasErrors())
            {
                foreach (var item in _service.Messages)
                    return BadRequest(item.Message);
            }

            return Ok(_service.Messages);
        }
    }
}