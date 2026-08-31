using Application.DTO.Request;
using Application.DTO.Response;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping;

public sealed class ConviteWorkspaceMapping(WorkspaceMapping workspaceMapping)
    : Mapper<ConviteWorkspace, CriarConviteWorkspaceRequest, ConviteWorkspaceResponse>
{
    public override ConviteWorkspaceResponse MapToDto(ConviteWorkspace entity)
        => new(
            workspaceMapping.MapToDto(entity.Workspace),
            entity.RemetenteCodigo,
            entity.ExpiraEm);

    public override ConviteWorkspace MapToEntity(CriarConviteWorkspaceRequest dto)
        => new()
        {
            WorkspaceId = dto.WorkspaceId,
            RemetenteCodigo = dto.RemetenteCodigo,
            IdentificadorHash = dto.IdentificadorHash,
            ExpiraEm = dto.ExpiraEm
        };
}
