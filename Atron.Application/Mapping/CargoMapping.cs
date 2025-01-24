using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class CargoMapping : ApplicationMapService<CargoDTO, Cargo>
    {
        public override CargoDTO MapToDTO(Cargo entity)
        {
            return new CargoDTO(entity.Codigo, entity.Descricao)
            {
                DepartamentoCodigo = entity.DepartamentoCodigo,
                Departamento = new DepartamentoDTO(entity.Departamento.Codigo, entity.Departamento.Descricao)
            };

        }

        public override Cargo MapToEntity(CargoDTO dto)
        {
            return new Cargo(dto.Codigo, dto.Descricao, dto.DepartamentoCodigo);
        }
    }
}
