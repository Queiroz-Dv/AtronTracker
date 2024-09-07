using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CargoController : ControllerBase
    {
        private readonly ICargoService _cargoService;

        public CargoController(ICargoService cargoService)
        {
            _cargoService = cargoService;
        }

        [HttpGet]
        [Route("ObterCargos")]
        public async Task<ActionResult<IEnumerable<CargoDTO>>> Get()
        {
            var cargos = await _cargoService.ObterTodosAsync();
            return Ok(cargos);
        }

        [HttpPost]
        [Route("CriarCargo")]
        public async Task<ActionResult> Post([FromBody] CargoDTO cargo)
        {
            await _cargoService.CriarAsync(cargo);

            return _cargoService.GetMessages().HasErrors() ?
                 BadRequest(ObterNotificacoes()) :
                 Ok(ObterNotificacoes());
        }

        [HttpPut("AtualizarCargo/{codigo}")]
        public async Task<ActionResult<CargoDTO>> Put(string codigo, [FromBody] CargoDTO cargo)
        {
            await _cargoService.AtualizarAsync(codigo, cargo);

            return _cargoService.GetMessages().HasErrors() ?
               BadRequest(ObterNotificacoes()) :
               Ok(ObterNotificacoes());
        }

        [HttpDelete("ExcluirCargo/{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _cargoService.RemoverAsync(codigo);

            return _cargoService.GetMessages().HasErrors() ?
            BadRequest(ObterNotificacoes()) :
            Ok(ObterNotificacoes());
        }

        [HttpGet]
        [Route("ObterPorCodigo/{codigo}")]
        public async Task<ActionResult<IEnumerable<CargoDTO>>> Get(string codigo)
        {
            var cargo = await _cargoService.ObterPorCodigoAsync(codigo);

            return cargo is null ?
               NotFound(ObterNotificacoes()) :
               Ok(cargo);
        }

        private IEnumerable<dynamic> ObterNotificacoes()
        {
            return _cargoService.GetMessages().ConvertMessageToJson();
        }
    }
}