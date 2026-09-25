using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class WorkspaceMapping(
        IToDtoMapper<Usuario, UsuarioDTO> usuarioMap
        ) : Mapper<Workspace, WorkspaceDTO>
    {

        public override WorkspaceDTO MapToDto(Workspace entity)
        {
            return new WorkspaceDTO()
            {
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                Responsavel = entity.Responsavel.MapToDto(usuarioMap)
            };
        }

        public override Workspace MapToEntity(WorkspaceDTO dto)
        {
            return new Workspace()
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                ResponsavelId = dto.Responsavel.Id,
                ResponsavelCodigo = dto.Responsavel.Codigo,
                ResponsavelEmail = dto.Responsavel.Email
            };
        }
    }
}
