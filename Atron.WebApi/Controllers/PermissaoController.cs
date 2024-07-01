using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Notification.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissaoController : ControllerBase
    {
        private IPermissaoService _permissaoService;

        public PermissaoController(IPermissaoService permissaoService)
        {
            _permissaoService = permissaoService;
        }

        [Route("CriarPermissao")]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PermissaoDTO permissao)
        {
            if (permissao is null)
            {
                return BadRequest(new NotificationMessage("Registro inválido, tente novamente"));
            }

            await _permissaoService.CriarPermissaoServiceAsync(permissao);
            return Ok(_permissaoService.Messages);
        }

        [Route("ObterPermissoes")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissaoDTO>>> Get()
        {
            var permissoes = await _permissaoService.ObterTodasPermissoesServiceAsync();

            if (permissoes is null)
            {
                return NotFound("Não foi encontrado nenhum registro");
            }

            return Ok(permissoes);
        }
    }
}
