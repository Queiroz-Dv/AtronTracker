using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerfilDeAcessoController : ApiBaseConfigurationController<PerfilDeAcessoDTO, IPerfilDeAcessoService>
    {
        private readonly IModuloService _moduloService;

        public PerfilDeAcessoController( 
            IModuloService moduloService,
            IPerfilDeAcessoService service,
            MessageModel messageModel) : base(service, messageModel)
        {
            _moduloService = moduloService;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.CriarPerfilServiceAsync(perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var perfis = await _service.ObterTodosPerfisServiceAsync();
            return Ok(perfis);
        }

        [HttpGet]
        [Route("ObterModulos")]
        public async Task<ActionResult> ObterModulos()
        {
            var modulos = await _moduloService.ObterTodosService();
            return Ok(modulos);
        }
    }
}
