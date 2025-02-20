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
                Departamento = new DepartamentoDTO
                {
                    Codigo = entity.Departamento.Codigo,
                    Descricao = entity.Departamento.Descricao
                }
            };
        }

        public override Cargo MapToEntity(CargoDTO dto)
        {
            return new Cargo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo.ToUpper()
            };
        }
    }
}
