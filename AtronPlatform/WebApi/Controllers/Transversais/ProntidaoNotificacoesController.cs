using AtronNotificacoes.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AtronPlatform.WebApi.Controllers.Transversais;

[ApiController]
[Authorize]
[Route("api/notificacoes")]
public sealed class ProntidaoNotificacoesController : ControllerBase
{
    [HttpGet("prontidao")]
    [ProducesResponseType(typeof(ProntidaoNotificacoesResponse), StatusCodes.Status200OK)]
    public ActionResult<ProntidaoNotificacoesResponse> ObterProntidao()
    {
        return Ok(new ProntidaoNotificacoesResponse(
            "AtronNotificacoes",
            NotificacoesResource.Status_Preparado));
    }
}

public sealed record ProntidaoNotificacoesResponse(string Modulo, string Status);
