using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class CargoMapping : AsyncApplicationMapService<CargoDTO, Cargo>
    {
        public override  Task<CargoDTO> MapToDTOAsync(Cargo entity)
        {
            var cargo = new CargoDTO(entity.Codigo, entity.Descricao)
            {
                DepartamentoCodigo = entity.DepartamentoCodigo,
                DepartamentoDescricao = entity.Departamento.Descricao,
            };

            return Task.FromResult(cargo);
        }

        public override Task<Cargo> MapToEntityAsync(CargoDTO dto)
        {
            return Task.FromResult(new Cargo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo.ToUpper()
            });
        }
    }
}
