using Domain.Entities;
using Application.DTO;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public sealed class ModuloMapping : Mapper<Modulo, ModuloDTO>
    {
        //TODO: Ainda será necessário verificar se as propriedades devem seguir junto com os módulos
        public override ModuloDTO MapToDto(Modulo entity)
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
