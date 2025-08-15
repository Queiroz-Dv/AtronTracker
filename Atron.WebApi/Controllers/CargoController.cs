using Atron.Application.DTO;
using Atron.Application.DTO.Request;
using Atron.Application.DTO.Response;
using Atron.Application.Extensions;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gerenciar entidades de cargo.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "Modulo:CRG")]
    public class CargoController : ApiBaseConfigurationController<Cargo, ICargoService>
    {
        /// <summary>
        /// Inicializa uma nova instância da classe <see cref="CargoController"/>.
        /// </summary>
        /// <param name="cargoService">O serviço para gerenciar departamentos.</param>
        /// <param name="serviceAccessor">O serviço de acesso para inicializar qualquer serviço necessário</param>
        /// <param name="messageModel">O modelo de mensagens para lidar com notificações.</param>
        public CargoController(ICargoService cargoService, IServiceAccessor serviceAccessor, MessageModel messageModel)
            : base(cargoService, serviceAccessor, messageModel)
        { }
        
        /// <summary>
        /// Obtém todos os cargos.
        /// </summary>
        /// <returns>Lista de cargos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CargoResponse>>> Get()
        {
            var cargos = await _service.ObterTodosAsync();

            return Ok(cargos.Select(x => x.MontarResponse()).ToList());
        }

        /// <summary>
        /// Cria um novo cargo.
        /// </summary>
        /// <param name="cargo">Dados do cargo a ser criado.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CargoRequest cargo)
        {
            await _service.CriarAsync(cargo.MontarDTO());
            return _messageModel.Notificacoes.HasErrors() ?
               BadRequest(ObterNotificacoes()) :
               Ok(ObterNotificacoes());
        }

        /// <summary>
        /// Atualiza um cargo existente.
        /// </summary>
        /// <param name="codigo">Código do cargo a ser atualizado.</param>
        /// <param name="cargo">Dados atualizados do cargo.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] CargoRequest cargo)
        {
            await _service.AtualizarAsync(codigo, cargo.MontarDTO());

            return _messageModel.Notificacoes.HasErrors() ?
               BadRequest(ObterNotificacoes()) :
               Ok(ObterNotificacoes());
        }

        /// <summary>
        /// Remove um cargo existente.
        /// </summary>
        /// <param name="codigo">Código do cargo a ser removido.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.RemoverAsync(codigo);

            return _messageModel.Notificacoes.HasErrors() ?
            BadRequest(ObterNotificacoes()) :
            Ok(ObterNotificacoes());
        }

        /// <summary>
        /// Obtém um cargo pelo código.
        /// </summary>
        /// <param name="codigo">Código do cargo.</param>
        /// <returns>Dados do departamento.</returns>
        [HttpGet("{codigo}")]
        public async Task<ActionResult<CargoResponse>> Get(string codigo)
        {
            var cargo = await _service.ObterPorCodigoAsync(codigo);
            return cargo is null ?
               NotFound(ObterNotificacoes()) :
               Ok(cargo.MontarResponse());
        }
    }
}