using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class CargoMapping : ApplicationMapService<CargoDTO, Cargo>
    {
        public override CargoDTO MapToDTO(Cargo entity)
        {
            var cargo = new CargoDTO
            {
                IdSequencial = entity.IdSequencial,
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                DepartamentoCodigo = entity.DepartamentoCodigo               
            };

            if (entity.Departamento != null)
            {
                cargo.Departamento = new DepartamentoDTO
                {
                    IdSequencial = entity.Departamento.IdSequencial,
                    Codigo = entity.Departamento.Codigo,
                    Descricao = entity.Departamento.Descricao
                };
            }

            return cargo;
        }

        public override Cargo MapToEntity(CargoDTO dto)
        {
            var cargo = new Cargo
            {
                IdSequencial = dto.IdSequencial,
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                DepartamentoCodigo = dto.DepartamentoCodigo
            };

           return cargo;
        }
    }
}
