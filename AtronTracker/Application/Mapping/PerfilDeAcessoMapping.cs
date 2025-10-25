using Application.DTO;
using Domain.Entities;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class PerfilDeAcessoMapping : AsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>
    {
        private readonly IAsyncApplicationMapService<ModuloDTO, Modulo> _moduloMap;

        public PerfilDeAcessoMapping(IAsyncApplicationMapService<ModuloDTO, Modulo> moduloMap) : base()
        {
            _moduloMap = moduloMap;
        }

        public override async Task<PerfilDeAcessoDTO> MapToDTOAsync(PerfilDeAcesso entity)
        {
            var dto = new PerfilDeAcessoDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                Modulos = []
            };

            if (entity.PerfilDeAcessoModulos != null)
            {

                foreach (var relacionamento in entity.PerfilDeAcessoModulos)
                {
                    if (relacionamento.Modulo != null)
                    {
                        var moduloDTO = await MapChildAsync(relacionamento.Modulo, _moduloMap);
                        dto.Modulos.Add(moduloDTO);
                    }
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