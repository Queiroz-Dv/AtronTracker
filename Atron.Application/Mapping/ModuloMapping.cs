using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Services.Mapper;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class ModuloMapping : AsyncApplicationMapService<ModuloDTO, Modulo>
    {
        //TODO: Ainda será necessário verificar se as propriedades devem seguir junto com os módulos
        public override Task<ModuloDTO> MapToDTOAsync(Modulo entity)
        {
            var moduloDTO = new ModuloDTO { Codigo = entity.Codigo, Descricao = entity.Descricao };            

            return Task.FromResult(moduloDTO);
        }

        public override Task<Modulo> MapToEntityAsync(ModuloDTO dto)
        {
            return Task.FromResult(new Modulo
            {
                Codigo = dto.Codigo.ToUpper(),
                Descricao = dto.Descricao.ToUpper()
            });
        }
    }
}