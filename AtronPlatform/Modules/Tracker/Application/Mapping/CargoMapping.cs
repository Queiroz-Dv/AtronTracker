using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class CargoMapping
        : Mapper<Cargo, CargoDTO>,
          IUpdateMapper<Cargo, CargoDTO>
    {
        public override CargoDTO MapToDto(Cargo entity)
        {
            return new CargoDTO(entity.Codigo, entity.Descricao)
            {
                Id = entity.Id,
                DepartamentoCodigo = entity.DepartamentoCodigo,
                DepartamentoDescricao = entity.Departamento?.Descricao,
                DepartamentoId = entity.DepartamentoId,
                Departamento = entity.Departamento != null ? new DepartamentoDTO
                {
                    Id = entity.Departamento.Id,
                    Codigo = entity.Departamento.Codigo,
                    Descricao = entity.Departamento.Descricao
                } : null
            };
        }

        public override Cargo MapToEntity(CargoDTO dto)
        {
            return new Cargo
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo
            };
        }

        public void MapToUpdate(CargoDTO dto, Cargo entityToUpdate)
        {
            entityToUpdate.Descricao = dto.Descricao.ToUpper();
            entityToUpdate.DepartamentoCodigo = dto.DepartamentoCodigo;
        }
    }
}
