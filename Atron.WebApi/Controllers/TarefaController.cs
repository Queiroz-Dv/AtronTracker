using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Application.Services;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaController : ControllerBase
    {
        private ITarefaService _service;

        public TarefaController(ITarefaService service)
        {
            _service = service;
        }

        [Route("CriarTarefa")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TarefaDTO tarefa)
        {
            if (tarefa is null)
            {
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));
            }

            await _service.CriarAsync(tarefa);

            return Ok(_service._messages);
        }

        [Route("ObterTarefas")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> Get()
        {
            var tarefas = await _service.ObterTodosAsync();

            if (tarefas is null)
            {
                return NotFound("Não foi encontrado nenhum registro");
            }

            return Ok(tarefas);
        }

        [HttpPut("AtualizarTarefa")]
        public async Task<ActionResult> Put([FromBody] TarefaDTO tarefa)
        {
            await _service.AtualizarAsync(tarefa);

            if (_service._messages.HasErrors())
            {
                foreach (var item in _service._messages)
                {
                    return BadRequest(item.Message);
                }
            }

            return Ok(_service._messages);
        }

        [HttpDelete("ExcluirTarefa")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                await _service.ExcluirAsync(id);

                if (_service._messages.HasErrors())
                {
                    foreach (var item in _service._messages)
                    {
                        return BadRequest(item.Message);
                    }
                }

                return Ok(_service._messages);
            }

            return BadRequest(new NotificationMessage("Identificador da permissão inválido", Notification.Enums.ENotificationType.Error));
        }
    }
}