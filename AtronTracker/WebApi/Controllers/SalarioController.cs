using Application.DTO;
using Application.DTO.Request;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:SAL")]
    public class SalarioController : ApiBaseConfigurationController<Salario, ISalarioService>
    {
        public SalarioController(ISalarioService service,
             IAccessorService serviceAccessor,
            Notifiable messageModel)
        : base(service, serviceAccessor, messageModel)
        { }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalarioDTO salario)
        {
            await _service.CriarAsync(salario);

            return _messageModel.Notificacoes.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalarioDTO>>> Get()
        {
            var salarios = await _service.ObterTodosAsync();
            return Ok(salarios.Select(s => s.MontarResponse()).ToList());
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalarioRequest salario)
        {
            await _service.AtualizarServiceAsync(id, salario.MontarDTO());

            return _messageModel.Notificacoes.HasErrors() ?
                 BadRequest(ObterNotificacoes()) : Ok(ObterNotificacoes());
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            await _service.ExcluirAsync(id.ToInt());

            return _messageModel.Notificacoes.HasErrors() ?
                    BadRequest(ObterNotificacoes()) :
                    Ok(ObterNotificacoes());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalarioDTO>> Get(int id)
        {
            var salario = await _service.ObterPorId(id);

            return salario is null ?
            NotFound(ObterNotificacoes()) :
            Ok(salario.MontarResponse());
        }
    }
}