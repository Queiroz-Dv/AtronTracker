using Application.DTO.Response;
using Application.UseCases.WorkspaceCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Filters;

namespace AtronPlatform.WebApi.Controllers.Tracker;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class WorkspaceController(
    CriarConviteWorkspaceCase criarConviteWorkspaceCase,
    ObterConviteWorkspaceCase obterConviteWorkspaceCase,
    AceitarConviteWorkspaceCase aceitarConviteWorkspaceCase,
    ObterWorkspacesUsuarioCase obterWorkspacesUsuarioCase,
    IUserAccessor userAccessor) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<WorkspaceInicialResponse>>> ObterMeusWorkspaces()
    {
        var resultado = await obterWorkspacesUsuarioCase.ExecutarAsync(
            userAccessor.ObterCodigoUsuarioLogado());

        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }

    [HttpPost("{workspaceId:int}/convites")]
    public async Task<ActionResult<ConviteWorkspaceCriadoResponse>> CriarConvite(
        int workspaceId)
    {
        var resultado = await criarConviteWorkspaceCase.ExecutarAsync(
            workspaceId,
            userAccessor.ObterCodigoUsuarioLogado());

        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }

    [AllowAnonymous]
    [HttpGet("convites/{identificador}")]
    public async Task<ActionResult<ConviteWorkspaceResponse>> ObterConvite(
        string identificador)
    {
        var resultado = await obterConviteWorkspaceCase.ExecutarAsync(identificador);

        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }

    [Transactional]
    [HttpPost("convites/{identificador}/aceitar")]
    public async Task<ActionResult<WorkspaceInicialResponse>> AceitarConvite(
        string identificador)
    {
        var resultado = await aceitarConviteWorkspaceCase.ExecutarAsync(
            identificador,
            userAccessor.ObterCodigoUsuarioLogado());

        return resultado.TeveFalha
            ? BadRequest(resultado.Messages)
            : Ok(resultado.Dados);
    }
}
