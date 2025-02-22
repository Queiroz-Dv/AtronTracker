using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class ModuloMapping : ApplicationMapService<ModuloDTO, Modulo>
    {
        public override ModuloDTO MapToDTO(Modulo entity)
        {
            return new ModuloDTO { Codigo = entity.Codigo, Descricao = entity.Descricao };
        }

        public override Modulo MapToEntity(ModuloDTO dto)
        {
            return new Modulo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper()
            };
        }
    }
}
