using AtronNotificacoes.Application.Interfaces;
using AtronNotificacoes.Contracts.DTO.Response;
using AtronNotificacoes.Resources;
using AtronNotificacoes.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronPlatform.WebApi.Controllers.Transversais;

[ApiController]
[Authorize(Policy = SegurancaNotificacoes.PoliticaUsuario)]
[Route("api/notificacoes")]
public sealed class NotificacoesInternasController(INotificacaoInternaService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<NotificacaoInternaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<NotificacaoInternaResponse>>> ObterMinhas()
    {
        return Ok(await service.ObterMinhasAsync(ObterDestinatarioCodigo()));
    }

    [HttpPost("{id:long}/marcar-como-lida")]
    [ProducesResponseType(typeof(NotificacaoInternaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<NotificacaoInternaResponse>> MarcarComoLida(long id)
    {
        var notificacao = await service.MarcarComoLidaAsync(id, ObterDestinatarioCodigo());
        return notificacao is null ? NotFound() : Ok(notificacao);
    }

    [HttpPost("marcar-todas-como-lidas")]
    [ProducesResponseType(typeof(IReadOnlyList<NotificacaoInternaResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<NotificacaoInternaResponse>>> MarcarTodasComoLidas()
    {
        return Ok(await service.MarcarTodasComoLidasAsync(ObterDestinatarioCodigo()));
    }

    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(long id)
    {
        var excluida = await service.ExcluirAsync(id, ObterDestinatarioCodigo());
        return excluida ? NoContent() : NotFound();
    }

    private string ObterDestinatarioCodigo()
    {
        return User.FindFirst(SegurancaNotificacoes.ClaimCodigoUsuario)?.Value
            ?? throw new UnauthorizedAccessException(
                NotificacoesResource.Erro_TokenSemCodigoUsuario);
    }
}
