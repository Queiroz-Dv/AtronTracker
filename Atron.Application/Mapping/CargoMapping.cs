using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class CargoMapping : ApplicationMapService<CargoDTO, Cargo>
    {
        public override CargoDTO MapToDTO(Cargo entity)
        {
            var cargo = new CargoDTO(entity.Codigo, entity.Descricao, entity.Departamento == null ? null : new DepartamentoDTO(entity.Departamento.Codigo, entity.Departamento.Descricao))
            {
                DepartamentoCodigo = entity.DepartamentoCodigo
            };
            return cargo;
        }

        public override Cargo MapToEntity(CargoDTO dto)
        {
            var cargo = new Cargo
            {                
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                DepartamentoCodigo = dto.DepartamentoCodigo
            };

            return cargo;
        }
    }
}
