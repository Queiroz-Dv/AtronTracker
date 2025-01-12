using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : ModuleController<Cargo, ICargoService>
    {
        public CargoController(ICargoService cargoService,
                               MessageModel<Cargo> messageModel)
            : base(cargoService, messageModel) { }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoDTO>>> Get()
        {
            var cargos = await _service.ObterTodosAsync();
            return Ok(cargos);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CargoDTO cargo)
        {
            await _service.CriarAsync(cargo);
            return RetornoPadrao(cargo);
        }        

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] CargoDTO cargo)
        {
            await _service.AtualizarAsync(codigo, cargo);

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

        [HttpGet]
        [Route("{codigo}")]
        public async Task<ActionResult<CargoDTO>> Get(string codigo)
        {
            var cargo = await _service.ObterPorCodigoAsync(codigo);

            return cargo is null ?
               NotFound(ObterNotificacoes()) :
               Ok(cargo);
        }
    }
}