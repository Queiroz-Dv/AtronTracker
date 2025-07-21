using Atron.Application.DTO;
using Atron.Application.DTO.Request;
using Atron.Application.DTO.Response;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    /// <summary>
    /// Controlador para gerenciar entidades de cargo.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "Modulo:CRG")]
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


            var cargosResponse = new List<CargoResponse>();
            foreach (var item in cargos)
            {
                cargosResponse.Add(new CargoResponse(item.Codigo,
                                                     item.Descricao,
                                                     item.DepartamentoCodigo,
                                                     item.DepartamentoDescricao));
            }

            return Ok(cargosResponse);
        }

        /// <summary>
        /// Cria um novo cargo.
        /// </summary>
        /// <param name="cargoRequest">Dados do cargo a ser criado.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CargoRequest cargoRequest)
        {
            CargoDTO cargo = MontarCargo(cargoRequest);
            await _service.CriarAsync(cargo);
            return _messageModel.Notificacoes.HasErrors() ?
               BadRequest(ObterNotificacoes()) :
               Ok(ObterNotificacoes());
        }

        private static CargoDTO MontarCargo(CargoRequest cargoRequest)
        {
            return new CargoDTO(cargoRequest.Codigo, cargoRequest.Descricao, cargoRequest.DepartamentoCodigo);
        }

        /// <summary>
        /// Atualiza um cargo existente.
        /// </summary>
        /// <param name="codigo">Código do cargo a ser atualizado.</param>
        /// <param name="cargoRequest">Dados atualizados do cargo.</param>
        /// <returns>Resultado da operação.</returns>
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] CargoRequest cargoRequest)
        {
            var cargo = MontarCargo(cargoRequest);
            await _service.AtualizarAsync(codigo, cargo);

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
            await _service.RemoverAsync(codigo.ToUpper());

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
               Ok(new CargoResponse(cargo.Codigo, cargo.Descricao, cargo.Codigo, cargo.DepartamentoDescricao));
        }
    }
}