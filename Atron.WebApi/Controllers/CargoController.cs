using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
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

            if (cargos is not null)
            {
                return Ok(cargos);
            }
            else
            {
                return NoContent();
            }
        }

        [HttpPost]
        [Route("CriarCargo")]
        public async Task<ActionResult> Post([FromBody] CargoDTO cargo)
        {
            if (cargo == null)
            {
                return BadRequest("Registro inválido, tente novamente");
            }

            await _cargoService.CriarAsync(cargo);

            return Ok(_cargoService.notificationMessages);
        }

        [HttpPut("AtualizarCargo/{codigo}")]
        public async Task<ActionResult<CargoDTO>> Put(string codigo, [FromBody] CargoDTO cargo)
        {
            if (codigo != cargo.Codigo || string.IsNullOrEmpty(codigo))
            {
                return BadRequest();
            }

            await _cargoService.AtualizarAsync(cargo);
            if (_cargoService.notificationMessages.HasErrors())
            {
                foreach (var item in _cargoService.notificationMessages)
                {
                    return BadRequest(item.Message);
                }
            }

            return Ok(_cargoService.notificationMessages);
        }

        [HttpDelete("ExcluirCargo/{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            var cargo = await _cargoService.ObterPorCodigoAsync(codigo);

            if (cargo is null)
            {
                return NotFound(new NotificationMessage("Cargo não encontrado"));
            }

            await _cargoService.RemoverAsync(cargo.Id);

            return Ok(cargo);
        }
    }
}