#nullable disable

using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class ObterWorkspaceAtualCase(
    IWorkspaceAtualService workspaceAtualService,
    IWorkspaceRepository workspaceRepository,
    WorkspaceMapping mapping)
{
    public async Task<WorkspaceInicialResponse> ExecutarAsync(string usuarioCodigo)
    {
        if (usuarioCodigo.IsNullOrEmpty())
            return null;

        var workspaceId = workspaceAtualService.ObterId();
        if (!workspaceId.HasValue)
            return null;

        var workspace = await workspaceRepository.ObterPorIdDoUsuarioAsync(workspaceId.Value, usuarioCodigo);

        if (!workspace.IsNullable())
            return mapping.MapToDto(workspace);

        workspaceAtualService.Remover();
        return null;
    }
}
