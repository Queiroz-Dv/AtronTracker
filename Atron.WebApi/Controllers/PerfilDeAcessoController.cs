using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PerfilDeAcessoController : ApiBaseConfigurationController<PerfilDeAcessoDTO, IPerfilDeAcessoService>
    {
        public PerfilDeAcessoController(
            IPerfilDeAcessoService service,
            MessageModel messageModel) : base(service, messageModel)
        { }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.CriarPerfilServiceAsync(perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }
    }
}
