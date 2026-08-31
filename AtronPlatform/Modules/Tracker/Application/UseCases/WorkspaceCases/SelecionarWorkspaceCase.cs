using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class SelecionarWorkspaceCase(
    IWorkspaceRepository workspaceRepository,
    IWorkspaceAtualService workspaceAtualService,
    WorkspaceMapping mapping)
{
    public async Task<Resultado<WorkspaceInicialResponse>> ExecutarAsync(
        SelecionarWorkspaceRequest request,
        string usuarioCodigo)
    {
        if (request.IsNullable() || request.WorkspaceId <= 0)
            return Resultado<WorkspaceInicialResponse>.Falha(
                WorkspaceResource.Erro_WorkspaceIdInvalido);

        if (usuarioCodigo.IsNullOrEmpty())
            return Resultado<WorkspaceInicialResponse>.Falha(WorkspaceResource.Erro_UsuarioCodigoObrigatorio);

        var workspace = await workspaceRepository.ObterPorIdDoUsuarioAsync(request.WorkspaceId, usuarioCodigo);
        if (workspace.IsNullable())
            return Resultado<WorkspaceInicialResponse>.Falha(WorkspaceResource.Erro_WorkspaceNaoEncontrado);


        workspaceAtualService.Definir(workspace.Id);

        return Resultado<WorkspaceInicialResponse>.Sucesso(mapping.MapToDto(workspace));
    }
}
