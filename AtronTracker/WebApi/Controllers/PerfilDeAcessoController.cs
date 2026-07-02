using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Extensions;
using System.Threading.Tasks;
using Shared.Domain.ValueObjects;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PerfilDeAcessoController : ApiBaseConfigurationController<PerfilDeAcessoDTO, IPerfilDeAcessoService>
    {
        public PerfilDeAcessoController(
            IPerfilDeAcessoService service,
            Notifiable messageModel) : base(service, messageModel)
        {
        }

        [HttpPost]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Post([FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.CriarPerfilServiceAsync(perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpPut("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Put(string codigo, [FromBody] PerfilDeAcessoDTO perfilDeAcessoDTO)
        {
            return !await _service.AtualizarPerfilServiceAsync(codigo, perfilDeAcessoDTO) ?
                     BadRequest(ObterNotificacoes()) :
                     Ok(ObterNotificacoes());
        }

        [HttpGet]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Get()
        {
            var perfis = await _service.ObterTodosPerfisServiceAsync();
            return Ok(perfis);
        }

        [HttpGet("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Get(string codigo)
        {
            var perfil = await _service.ObterPerfilPorCodigoServiceAsync(codigo);
            return Ok(perfil);
        }

        [HttpDelete("{codigo}")]
        [Authorize(Policy = ModuloPolicies.PerfilDeAcesso)]
        public async Task<ActionResult> Delete(string codigo)
        {
            await _service.DeletarPerfilServiceAsync(codigo);

            return _messageModel.Notificacoes.HasErrors() ?
                    BadRequest(ObterNotificacoes()) :
                    Ok(ObterNotificacoes());
        }

        [HttpPost]
        [Route("RelacionamentoPerfilUsuario")]
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
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
        [Authorize(Policy = ModuloPolicies.RelacionamentoPerfilUsuario)]
        public async Task<ActionResult> ObterRelacionamentosPerfilUsuario([FromRoute] string codigo)
        {
            var perfil = await _service.ObterRelacionamentoDePerfilUsuarioPorCodigoServiceAsync(codigo);
            return Ok(perfil);
        }
    }

    public class UsuarioPerfilRequest
    {
        public string Codigo { get; set; }
    }

    public class PerfilUsuarioRequest
    {
        public string CodigoPerfil { get; set; }
        public UsuarioPerfilRequest[] Usuarios { get; set; }
    }
}
