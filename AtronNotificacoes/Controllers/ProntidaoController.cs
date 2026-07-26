using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronNotificacoes.Controllers;

[ApiController]
[Authorize]
[Route("api/notificacoes")]
public sealed class ProntidaoController : ControllerBase
{
    [HttpGet("prontidao")]
    [ProducesResponseType(typeof(ProntidaoNotificacoesResponse), StatusCodes.Status200OK)]
    public ActionResult<ProntidaoNotificacoesResponse> ObterProntidao()
    {
        return Ok(new ProntidaoNotificacoesResponse("AtronNotificacoes", "Preparado"));
    }
}

public sealed record ProntidaoNotificacoesResponse(string Modulo, string Status);
