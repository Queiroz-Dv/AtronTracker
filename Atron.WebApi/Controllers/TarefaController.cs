using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using Shared.Extensions;
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
            await _service.CriarAsync(tarefa);

            return _messageModel.Messages.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(string id, [FromBody] TarefaDTO tarefa)
        {
            await _service.AtualizarAsync(tarefa);

            return _messageModel.Messages.HasErrors() ?
                 BadRequest(ObterNotificacoes()) : Ok(ObterNotificacoes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.ExcluirAsync(id);

            return _messageModel.Messages.HasErrors() ?
                    BadRequest(ObterNotificacoes()) :
                    Ok(ObterNotificacoes());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TarefaDTO>> Get(int id)
        {
            var tarefa = await _service.ObterPorId(id);

            return tarefa is null ?
            NotFound(ObterNotificacoes()) :
            Ok(tarefa);
        }
    }
}