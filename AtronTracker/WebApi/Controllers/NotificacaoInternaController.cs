using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacaoInternaController(INotificacaoInternaService notificacaoInternaService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacaoInternaDTO>>> ObterMinhas()
        {
            var resultado = await notificacaoInternaService.ObterMinhasAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("{id}/MarcarComoLida")]
        public async Task<ActionResult<NotificacaoInternaDTO>> MarcarComoLida(int id)
        {
            var resultado = await notificacaoInternaService.MarcarComoLidaAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        [HttpPost("MarcarTodasComoLidas")]
        public async Task<ActionResult<IEnumerable<NotificacaoInternaDTO>>> MarcarTodasComoLidas()
        {
            var resultado = await notificacaoInternaService.MarcarTodasComoLidasAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
