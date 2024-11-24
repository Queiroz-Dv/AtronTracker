using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TarefaController : ModuleController<Tarefa, ITarefaService>
    {
        public TarefaController(ITarefaService service, MessageModel<Tarefa> messageModel) : 
            base(service, messageModel)
        { }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaDTO>>> Get()
        {
            var tarefas = await _service.ObterTodosAsync();

            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] TarefaDTO tarefa)
        {
            if (tarefa is null)
            {
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));
            }

            await _service.CriarAsync(tarefa);

            return Ok(_service.Messages);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] TarefaDTO tarefa)
        {
            await _service.AtualizarAsync(tarefa);

            if (_service.Messages.HasErrors())
            {
                foreach (var item in _service.Messages)
                {
                    return BadRequest(item.Message);
                }
            }

            return Ok(_service.Messages);
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                await _service.ExcluirAsync(id);

                if (_service.Messages.HasErrors())
                {
                    foreach (var item in _service.Messages)
                    {
                        return BadRequest(item.Message);
                    }
                }

                return Ok(_service.Messages);
            }

            return BadRequest(new NotificationMessage("Identificador da permissão inválido", Notification.Enums.ENotificationType.Error));
        }
    }
}