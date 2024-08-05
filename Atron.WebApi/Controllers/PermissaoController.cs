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
            return Ok(_permissaoService._messages);
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

        [HttpPut("AtualizarPermissao")]
        public async Task<ActionResult<PermissaoDTO>> Put([FromBody] PermissaoDTO permissao)
        {
            await _permissaoService.AtualizarPermissaoServiceAsync(permissao);

            if (_permissaoService._messages.HasErrors())
            {
                foreach (var item in _permissaoService._messages)
                {
                    return BadRequest(item.Message);
                }
            }

            return Ok(_permissaoService._messages);
        }

        // TODO: Testar depois
        [HttpDelete("ExcluirPermissao")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id != 0)
            {
                await _permissaoService.ExcluirPermissaoServiceAsync(id);

                if (_permissaoService._messages.HasErrors())
                {
                    foreach (var item in _permissaoService._messages)
                    {
                        return BadRequest(item.Message);
                    }
                }

                return Ok(_permissaoService._messages);
            }

            return BadRequest(new NotificationMessage("Identificador da permissão inválido", Notification.Enums.ENotificationType.Error));
        }
    }
}