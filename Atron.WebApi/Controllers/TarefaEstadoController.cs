using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TarefaEstadoController : ModuleController<TarefaEstado, ITarefaEstadoService>
    {
        //TODO: Sempre que tenho uma entidade simplificada sou forçado a ter um message model dela para validação
        // Necessário criar uma forma de não ter um messageModel 
        // Necessário rever a alteração da regra para que os estados de uma tarefa façam parte da tabela de tarefas
        public TarefaEstadoController(ITarefaEstadoService service,
            MessageModel messageModel) :
            base(service, messageModel)
        { }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TarefaEstado>>> Get()
        {
            var estados = await _service.ObterTodosServiceAsync();
            return Ok(estados);
        }
    }
}
