using Application.DTO;
using Application.Interfaces.Mapping;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;
using System.Linq;

namespace Application.Mapping
{
    public sealed class PerfilDeAcessoMapping
        : Mapper<PerfilDeAcesso, PerfilDeAcessoDTO>, IPerfilDeAcessoMapping
    {
        private readonly IToDtoMapper<Modulo, ModuloDTO> _moduloMap;
        private readonly IToDtoMapper<PerfilDeAcessoUsuario, UsuarioDTO> _usuarioMap;

        public PerfilDeAcessoMapping(
            IToDtoMapper<Modulo, ModuloDTO> moduloMap,
            IToDtoMapper<PerfilDeAcessoUsuario, UsuarioDTO> usuarioMap)
        {
            _moduloMap = moduloMap;
            _usuarioMap = usuarioMap;
        }

        public override PerfilDeAcessoDTO MapToDto(PerfilDeAcesso entity)
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
                        var moduloDTO = relacionamento.Modulo.MapToDto(_moduloMap);
                        dto.Modulos.Add(moduloDTO);
                    }
                }
            }

            return dto;
        }

        public PerfilDeAcessoUsuarioDTO MapToPerfilDeAcessoUsuarioDto(PerfilDeAcesso perfilDeAcesso)
        {
            var perfilDeAcessoDTO = MapToDto(perfilDeAcesso);
            var usuarios = _usuarioMap
                .MapToDtos(perfilDeAcesso.PerfisDeAcessoUsuario ?? [])
                .ToList();

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

        public override PerfilDeAcesso MapToEntity(PerfilDeAcessoDTO dto)
        {
            return new PerfilDeAcesso
            {
                Codigo = dto.Codigo,
                Descricao = dto.Descricao
            };
        }
    }
}
