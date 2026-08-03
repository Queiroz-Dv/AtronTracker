using Application.DTO;
using Application.Interfaces.Mapping;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Mapping
{
    public class PerfilDeAcessoMapping : AsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>, IPerfilDeAcessoMapping
    {
        private readonly IAsyncApplicationMapService<ModuloDTO, Modulo> _moduloMap;
        private readonly IAsyncApplicationMapService<UsuarioDTO, PerfilDeAcessoUsuario> _usuarioMap;

        public PerfilDeAcessoMapping(
            IAsyncApplicationMapService<ModuloDTO, Modulo> moduloMap,
            IAsyncApplicationMapService<UsuarioDTO, PerfilDeAcessoUsuario> usuarioMap,
            IMapperEngineService mapperEngine) : base(mapperEngine)
        {
            _moduloMap = moduloMap;
            _usuarioMap = usuarioMap;
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

        public async Task<PerfilDeAcessoUsuarioDTO> MapToPerfilDeAcessoUsuarioDTOAsync(PerfilDeAcesso perfilDeAcesso)
        {
            var perfilDeAcessoDTO = await MapToDTOAsync(perfilDeAcesso);
            var usuarios = await MapChildrenAsync(
                perfilDeAcesso.PerfisDeAcessoUsuario ?? [],
                _usuarioMap);

            return new PerfilDeAcessoUsuarioDTO
            {
                PerfilDeAcesso = new PerfilDeAcessoDTO
                {
                    Codigo = perfilDeAcessoDTO.Codigo,
                    Descricao = perfilDeAcessoDTO.Descricao,
                    Modulos = perfilDeAcessoDTO.Modulos.ToList()
                },
                Usuarios = usuarios
            };
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
