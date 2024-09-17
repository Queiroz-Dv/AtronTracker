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
    [Produces("application/json")]
    [ApiController]
    public class DepartamentoController : ModuleController<Departamento, IDepartamentoService>
    {
        public DepartamentoController(IDepartamentoService departamentoService, MessageModel<Departamento> messageModel)
            : base(departamentoService, messageModel)
        {
            // Injeta a dependência do serviço de departamento no construtor
            // Porém aqui não é necessário pois a controller de módulos já faz automaticamente
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DepartamentoDTO departamento)
        {            
            await _service.CriarAsync(departamento);

            return _messageModel.Messages.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()
        {
            var departamentos = await _service.ObterTodosAsync();
            return Ok(departamentos);
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            await _service.AtualizarAsync(codigo, departamento);

            return _messageModel.Messages.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.RemoverAsync(codigo);

            return _messageModel.Messages.HasErrors() ?
                BadRequest(ObterNotificacoes()) :
                Ok(ObterNotificacoes());
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var departamento = await _service.ObterPorCodigo(codigo);

            return departamento is null ?  
                NotFound(ObterNotificacoes()) :  
                Ok(departamento);            
        }       
    }
}