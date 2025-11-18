using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using Shared.Extensions;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shared.Application.Interfaces.Service;

namespace WebApi.Controllers
{
    /// <summary>  
    /// Controlador para gerenciar entidades de Departamento.  
    /// </summary>  
    [Authorize(Policy = "Modulo:DPT")] // Configura a policy para o módulo de Departamento
    [ApiController]
    [Route("api/[controller]")]
    public class DepartamentoController : ApiBaseConfigurationController<Departamento, IDepartamentoService>
    {
        /// <summary>  
        /// Inicializa uma nova instância da classe <see cref="DepartamentoController"/>.  
        /// </summary>  
        /// <param name="departamentoService">O serviço para gerenciar departamentos.</param>
        /// <param name="serviceAccessor">O serviço de acesso para inicializar qualquer serviço necessário</param>
        /// <param name="messageModel">O modelo de mensagens para lidar com notificações.</param>  
        public DepartamentoController(IDepartamentoService departamentoService, IAccessorService serviceAccessor, MessageModel messageModel)
            : base(departamentoService, serviceAccessor, messageModel)
        { }

        /// <summary>  
        /// Cria um novo departamento.  
        /// </summary>  
        /// <param name="departamento">Dados do departamento a ser criado.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] DepartamentoDTO departamento)
        {
            await _service.CriarAsync(departamento);

            return _messageModel.Notificacoes.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        /// <summary>  
        /// Obtém todos os departamentos.  
        /// </summary>  
        /// <returns>Lista de departamentos.</returns>  
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()
        {
            var departamentos = await _service.ObterTodosAsync();
            return Ok(departamentos);
        }

        /// <summary>  
        /// Atualiza um departamento existente.  
        /// </summary>  
        /// <param name="codigo">Código do departamento a ser atualizado.</param>  
        /// <param name="departamento">Dados atualizados do departamento.</param>  
        /// <returns>Resultado da operação.</returns>  
        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] DepartamentoDTO departamento)
        {
            await _service.AtualizarAsync(codigo, departamento);

            return _messageModel.Notificacoes.HasErrors() ?
                   BadRequest(ObterNotificacoes()) :
                   Ok(ObterNotificacoes());
        }

        /// <summary>  
        /// Remove um departamento existente.  
        /// </summary>  
        /// <param name="codigo">Código do departamento a ser removido.</param>  
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
        /// Obtém um departamento pelo código.  
        /// </summary>  
        /// <param name="codigo">Código do departamento.</param>  
        /// <returns>Dados do departamento.</returns>  
        [HttpGet("{codigo}")]
        public async Task<ActionResult<DepartamentoDTO>> Get(string codigo)
        {
            var departamento = await _service.ObterPorCodigo(codigo);

            return departamento is null ?
                NotFound(ObterNotificacoes()) :
                Ok(departamento);
        }
    }
}