using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class TarefaEstadoMapping : Mapper<TarefaEstado, TarefaEstadoDTO>
    {
        public override TarefaEstadoDTO MapToDto(TarefaEstado entity)
        {
            return new TarefaEstadoDTO(entity.Id, entity.Descricao);
        }

        public override TarefaEstado MapToEntity(TarefaEstadoDTO dto)
        {
            return new TarefaEstado(dto.Id, dto.Descricao);
        }
    }
}