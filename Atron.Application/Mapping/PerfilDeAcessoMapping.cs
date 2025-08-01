using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class PerfilDeAcessoMapping : AsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IModuloRepository moduloRepository;

        public PerfilDeAcessoMapping(IModuloRepository moduloRepository)
        {
            this.moduloRepository = moduloRepository;
        }

        public override async Task<PerfilDeAcessoDTO> MapToDTOAsync(PerfilDeAcesso entity)
        {
            var dto = new PerfilDeAcessoDTO() { Id = entity.Id, Codigo = entity.Codigo, Descricao = entity.Descricao, };

            dto.Modulos = new List<ModuloDTO>();
            if (entity.PerfilDeAcessoModulos != null)
            {
                foreach (var item in entity.PerfilDeAcessoModulos)
                {
                    var modulo = await moduloRepository.ObterPorCodigoRepository(item.ModuloCodigo);                 
                    var moduloDTO = new ModuloDTO()
                    {
                        Codigo = modulo.Codigo,
                        Descricao = modulo.Descricao
                    };

                    dto.Modulos.Add(moduloDTO);
                }
            }

            return dto;
        }

        public override Task<PerfilDeAcesso> MapToEntityAsync(PerfilDeAcessoDTO dto)
        {
            return Task.FromResult(new PerfilDeAcesso()
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao
            });
        }
    }
}