using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class CargoMapping : AsyncApplicationMapService<CargoDTO, Cargo>
    {
        private readonly IAsyncApplicationMapService<DepartamentoDTO, Departamento> _departamentoMap;

        public CargoMapping(IAsyncApplicationMapService<DepartamentoDTO, Departamento> departamentoMap) : base()
        {
            _departamentoMap = departamentoMap;
        }

        public override  async Task<CargoDTO> MapToDTOAsync(Cargo entity)
        {
            var cargo = new CargoDTO(entity.Codigo, entity.Descricao)
            {
                DepartamentoCodigo = entity.DepartamentoCodigo,
                DepartamentoDescricao = entity.Departamento?.Descricao,

                Departamento = await MapChildAsync(entity.Departamento, _departamentoMap)
            };

            return cargo;
        }

        public override Task<Cargo> MapToEntityAsync(CargoDTO dto)
        {
            var entity = new Cargo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper(),
                DepartamentoCodigo = dto.DepartamentoCodigo.ToUpper()
            };

            return Task.FromResult(entity);
        }
    }
}
