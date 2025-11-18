using Application.DTO;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Application.Interfaces.Service;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:TAR")]
    public class TarefaController : ApiBaseConfigurationController<Tarefa, ITarefaService>
    {
        public TarefaController(ITarefaService service, IAccessorService serviceAccessor, MessageModel messageModel) :
            base(service, serviceAccessor, messageModel)
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

            return _messageModel.Notificacoes.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] TarefaDTO tarefa)
        {
            await _service.AtualizarAsync(id, tarefa);

            return _messageModel.Notificacoes.HasErrors() ?
                 BadRequest(ObterNotificacoes()) : Ok(ObterNotificacoes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.ExcluirAsync(id);

            return _messageModel.Notificacoes.HasErrors() ?
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