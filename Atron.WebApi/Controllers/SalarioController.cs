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
    public class SalarioController : ControllerBase
    {
        private readonly ISalarioService _service;

        public SalarioController(ISalarioService service)
        {
            _service = service;
        }

        [Route("CriarSalario")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SalarioDTO salario)
        {
            if (salario is null)
            {
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));
            }

            await _service.CriarAsync(salario);

            return Ok(_service.Messages);
        }

        [Route("ObterSalarios")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalarioDTO>>> Get()
        {
            var salarios = await _service.ObterTodosAsync();
            if (salarios is null)
            {
                NotFound("Não foi encontrado nenum registro");
            }

            return Ok(salarios);
        }
    }
}
