using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gerenciar entidades de Módulos.
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ModuloController : ApiBaseConfigurationController<ModuloDTO, IModuloService>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="ModuloController"/>.
        /// </summary>
        /// <param name="service">O serviço para gerenciar módulos.</param>
        /// <param name="messageModel">O modelo de mensagens para lidar com notificações.</param>
        public ModuloController(IModuloService service, MessageModel messageModel) : base(service, messageModel)
        { }

        /// <summary>
        /// Obtém todos os módulos.
        /// </summary>
        /// <returns>Lista de módulos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuloDTO>>> Get()
        {
            var dtos = await _service.ObterTodosService();
            return Ok(dtos);
        }

        /// <summary>
        /// Obtém um módulo pelo código.
        /// </summary>
        /// <param name="codigo">Código do módulo.</param>
        /// <returns>Dados do módulo.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var modulo = await _service.ObterPorCodigoService(codigo);

            return modulo is null ?
                NotFound(ObterNotificacoes()) :
                Ok(modulo);
        }
    }
}