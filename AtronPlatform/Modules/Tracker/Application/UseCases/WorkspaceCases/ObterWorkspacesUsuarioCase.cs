using Application.DTO.Response;
using Application.Mapping;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class ObterWorkspacesUsuarioCase(
    IWorkspaceRepository workspaceRepository,
    WorkspaceMapping mapping)
{
    public async Task<Resultado<IReadOnlyCollection<WorkspaceInicialResponse>>> ExecutarAsync(
        string usuarioCodigo)
    {
        if (usuarioCodigo.IsNullOrEmpty())
        {
            return Resultado<IReadOnlyCollection<WorkspaceInicialResponse>>.Falha(
                WorkspaceResource.Erro_UsuarioCodigoObrigatorio);
        }

        var workspaces = await workspaceRepository.ObterPorUsuarioAsync(usuarioCodigo);
        var  response = workspaces.Select(mapping.MapToDto).ToArray();

        return Resultado<IReadOnlyCollection<WorkspaceInicialResponse>>.Sucesso(response);
    }
}