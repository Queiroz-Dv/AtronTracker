using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("Modulo:PERF")]    
    public class PerfilDeAcessoController : ApiBaseConfigurationController<PerfilDeAcessoDTO, IPerfilDeAcessoService>
    {
        public PerfilDeAcessoController(
            IPerfilDeAcessoService service,
            MessageModel messageModel) : base(service, messageModel)
        {
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.CriarPerfilServiceAsync(perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpPut("{codigo}")]
        public async Task<ActionResult> Put(string codigo, [FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.AtualizarPerfilServiceAsync(codigo, perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var perfis = await _service.ObterTodosPerfisServiceAsync();
            return Ok(perfis);
        }

        [HttpGet("{codigo}")]
        public async Task<ActionResult> Get(string codigo)
        {
            var perfil = await _service.ObterPerfilPorCodigoServiceAsync(codigo);
            return Ok(perfil);
        }

        [HttpDelete("{codigo}")]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.DeletarPerfilServiceAsync(codigo);

            return _messageModel.Messages.HasErrors() ?
                    BadRequest(ObterNotificacoes()) :
                    Ok(ObterNotificacoes());
        }

        [HttpPost]
        [Route("RelacionamentoPerfilUsuario")]
        public async Task<ActionResult> RelacionamentoPerfilUsuario(PerfilUsuarioRequest request)
        {
            var dto = new PerfilDeAcessoUsuarioDTO();
            dto.PerfilDeAcesso.Codigo = request.CodigoPerfil;
            foreach (var usr in request.Usuarios)
            {
                dto.Usuarios.Add(new UsuarioDTO() { Codigo = usr.Codigo });
            }


            return !await _service.RelacionarPerfilDeAcessoUsuarioServiceAsync(dto) ?
                      BadRequest(ObterNotificacoes()) :
                      Ok(ObterNotificacoes());
        }

        [HttpGet]
        [Route("ObterRelacionamentoPerfilUsuario/{codigo}")]
        public async Task<ActionResult> ObterRelacionamentosPerfilUsuario([FromRoute]string codigo)
        {
            var perfil = await _service.ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(codigo);
            return Ok(perfil);
        }
    }

    public class UsuarioRequest
    {
        public string Codigo { get; set; }
    }

    public class PerfilUsuarioRequest
    {
        public string CodigoPerfil { get; set; }
        public UsuarioRequest[] Usuarios { get; set; }
    }
}
