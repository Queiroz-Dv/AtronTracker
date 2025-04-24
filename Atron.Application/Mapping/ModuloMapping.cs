using Atron.Application.DTO;
using Atron.Application.DTO.ControleDeAcessoDTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class ModuloMapping : ApplicationMapService<ModuloDTO, Modulo>
    {
        //TODO: Ainda será necessário verificar se as propriedades devem seguir junto com os módulos
        public override ModuloDTO MapToDTO(Modulo entity)
        {
            var moduloDTO = new ModuloDTO { Codigo = entity.Codigo, Descricao = entity.Descricao };            

            return moduloDTO;
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
