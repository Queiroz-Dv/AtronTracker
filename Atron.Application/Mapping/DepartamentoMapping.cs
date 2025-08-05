using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class DepartamentoMapping : AsyncApplicationMapService<DepartamentoDTO, Departamento>
    {       
        public override Task<DepartamentoDTO> MapToDTOAsync(Departamento entity)
        {
            var dto = new DepartamentoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo.ToUpper(),
                Descricao = entity.Descricao.ToUpper()
            };

            return Task.FromResult(dto);
        }

        public override Task<Departamento> MapToEntityAsync(DepartamentoDTO dto)
        {
            var entity = new Departamento
            {
                Id = dto.Id,
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper()
            };
            return Task.FromResult(entity);
        }              
    }
}