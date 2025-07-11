﻿using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:SAL")]
    public class SalarioController : ApiBaseConfigurationController<Salario, ISalarioService>
    {
        public SalarioController(ISalarioService service,
             IServiceAccessor serviceAccessor,
            MessageModel messageModel)
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

            return Ok(salarios);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] SalarioDTO salario)
        {
            await _service.AtualizarServiceAsync(id, salario);

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
        public async Task<ActionResult<SalarioDTO>> Get(int id)
        {
            var salario = await _service.ObterPorId(id);

            return salario is null ?
            NotFound(ObterNotificacoes()) :
            Ok(salario);
        }
    }
}