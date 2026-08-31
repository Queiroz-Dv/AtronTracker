using Application.DTO.Request;
using Application.DTO.Response;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping;

public sealed class WorkspaceMapping
    : Mapper<Workspace, CriarWorkspaceInicialRequest, WorkspaceInicialResponse>
{
    public override WorkspaceInicialResponse MapToDto(Workspace entity)
        => new(
            entity.Id,
            entity.Nome,
            entity.Tipo,
            entity.Empresa?.Codigo);

    public override Workspace MapToEntity(CriarWorkspaceInicialRequest dto)
    {
        var workspace = new Workspace
        {
            Nome = dto.Nome,
            Tipo = dto.Tipo,
            EmpresaCodigo = dto.EmpresaCodigo
        };

        workspace.Membros.Add(new MembroWorkspace
        {
            UsuarioCodigo = dto.UsuarioCodigo,
            Workspace = workspace
        });

        return workspace;
    }
}
