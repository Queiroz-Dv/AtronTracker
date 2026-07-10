using Application.DTO;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de notificações internas do usuário autenticado.
    /// Permite listar, marcar individualmente ou marcar todas as notificações como lidas.
    /// Requer autorização (Bearer token).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificacaoInternaController(INotificacaoInternaService notificacaoInternaService) : ControllerBase
    {
        /// <summary>
        /// Obtém todas as notificações internas do usuário autenticado.
        /// </summary>
        /// <returns>200 OK com a lista de notificações ou 400 BadRequest com mensagens de erro.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotificacaoInternaDTO>>> ObterMinhas()
        {
            var resultado = await notificacaoInternaService.ObterMinhasAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Marca uma notificação interna específica como lida.
        /// </summary>
        /// <param name="id">Identificador numérico da notificação a ser marcada como lida.</param>
        /// <returns>200 OK com o DTO da notificação atualizada ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("{id}/MarcarComoLida")]
        public async Task<ActionResult<NotificacaoInternaDTO>> MarcarComoLida(int id)
        {
            var resultado = await notificacaoInternaService.MarcarComoLidaAsync(id);
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }

        /// <summary>
        /// Marca todas as notificações internas do usuário autenticado como lidas.
        /// </summary>
        /// <returns>200 OK com a lista de notificações atualizadas ou 400 BadRequest com mensagens de erro.</returns>
        [HttpPost("MarcarTodasComoLidas")]
        public async Task<ActionResult<IEnumerable<NotificacaoInternaDTO>>> MarcarTodasComoLidas()
        {
            var resultado = await notificacaoInternaService.MarcarTodasComoLidasAsync();
            return resultado.TeveFalha ? BadRequest(resultado.Messages) : Ok(resultado.Dados);
        }
    }
}
