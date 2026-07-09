using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ModuloController : ControllerBase
    {
        private readonly IModuloService _service;

        public ModuloController(IModuloService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuloDTO>>> Get()
        {
            var dtos = await _service.ObterTodosService();
            return Ok(dtos);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult<ModuloDTO>> Get(string codigo)
        {
            var modulo = await _service.ObterPorCodigoService(codigo);
            return modulo is null ? NotFound() : Ok(modulo);
        }
    }
}
