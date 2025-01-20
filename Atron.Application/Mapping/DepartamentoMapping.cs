using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class DepartamentoMapping : ApplicationMapService<DepartamentoDTO, Departamento>
    {
        public override DepartamentoDTO MapToDTO(Departamento entity)
        {
            return new DepartamentoDTO() { Codigo = entity.Codigo, Descricao = entity.Descricao };
        }

        public override Departamento MapToEntity(DepartamentoDTO dto)
        {
            return new Departamento() { Codigo = dto.Codigo, Descricao = dto.Descricao };
        }
    }
}