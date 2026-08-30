using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Mapping
{
    public class UsuarioIdentityMapping : Mapper<UsuarioIdentity, UsuarioDTO>
    {
        private readonly IToDtoMapper<Cargo, CargoDTO> _cargoMap;
        private readonly IToDtoMapper<Departamento, DepartamentoDTO> _departamentoMap;
        private readonly IToDtoMapper<PerfilDeAcesso, PerfilDeAcessoDTO> _perfilDeAcessoMap;

        public UsuarioIdentityMapping(
            IToDtoMapper<Cargo, CargoDTO> cargoMap,
            IToDtoMapper<Departamento, DepartamentoDTO> departamentoMap,
            IToDtoMapper<PerfilDeAcesso, PerfilDeAcessoDTO> perfilDeAcessoMap)
        {
            _cargoMap = cargoMap;
            _departamentoMap = departamentoMap;
            _perfilDeAcessoMap = perfilDeAcessoMap;
        }

        public override UsuarioDTO MapToDto(UsuarioIdentity entity)
        {
            var usuario = new UsuarioDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                Email = entity.Email,
                DataNascimento = entity.DataNascimento,
                PerfisDeAcesso = []
            };

            if (entity.UsuarioCargoDepartamentos != null)
            {
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    usuario.CargoCodigo = item.CargoCodigo;
                    usuario.Cargo = item.Cargo.MapToDto(_cargoMap);

                    usuario.DepartamentoCodigo = item.DepartamentoCodigo;
                    usuario.Departamento = item.Departamento.MapToDto(_departamentoMap);
                }
            }

            if (entity.PerfisDeAcessoUsuario != null)
            {
                foreach (var item in entity.PerfisDeAcessoUsuario)
                {
                    var perfilDeAcesso = item.PerfilDeAcesso.MapToDto(_perfilDeAcessoMap);
                    usuario.PerfisDeAcesso.Add(perfilDeAcesso);
                }
            }

            return usuario;
        }

        public override UsuarioIdentity MapToEntity(UsuarioDTO dto)
        {
            return new UsuarioIdentity
            {
                Codigo = dto.Codigo,
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email,
                DataNascimento = dto.DataNascimento
            };
        }
    }
}
