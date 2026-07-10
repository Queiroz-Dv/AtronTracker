using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller para consulta de módulos do sistema.
    /// Módulos representam funcionalidades que podem ser associadas a perfis de acesso.
    /// Requer autorização (Bearer token).
    /// </summary>
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

        /// <summary>
        /// Obtém todos os módulos cadastrados no sistema.
        /// </summary>
        /// <returns>200 OK com a lista de módulos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuloDTO>>> Get()
        {
            var dtos = await _service.ObterTodosService();
            return Ok(dtos);
        }

        /// <summary>
        /// Obtém um módulo pelo código.
        /// </summary>
        /// <param name="codigo">Código do módulo a ser consultado.</param>
        /// <returns>200 OK com os dados do módulo ou 404 NotFound se não encontrado.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<ModuloDTO>> Get(string codigo)
        {
            var modulo = await _service.ObterPorCodigoService(codigo);
            return modulo is null ? NotFound() : Ok(modulo);
        }
    }
}
