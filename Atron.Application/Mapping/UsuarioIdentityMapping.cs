using Atron.Application.DTO;
using Atron.Tracker.Domain.Entities;
using Shared.DTO.API;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class UsuarioIdentityMapping : AsyncApplicationMapService<UsuarioDTO, UsuarioIdentity>
    {
        private readonly IAsyncApplicationMapService<CargoDTO, Cargo> _cargoMap;
        private readonly IAsyncApplicationMapService<DepartamentoDTO, Departamento> _departamentoMap;
        private readonly IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> _perfilDeAcessoMap;

        public UsuarioIdentityMapping(IAsyncApplicationMapService<CargoDTO, Cargo> cargoMap,
                              IAsyncApplicationMapService<DepartamentoDTO, Departamento> departamentoMap,
                              IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso> perfilDeAcessoMap) : base()
        {
            _cargoMap = cargoMap;
            _departamentoMap = departamentoMap;
            _perfilDeAcessoMap = perfilDeAcessoMap;
        }

        public override async Task<UsuarioDTO> MapToDTOAsync(UsuarioIdentity entity)
        {
            var usuario = new UsuarioDTO
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                Email = entity.Email,
                Salario = entity.SalarioAtual,
                DataNascimento = entity.DataNascimento,
                DadosDoToken = new DadosDeTokenComRefreshToken()
                {
                    TokenDTO = new DadosDoTokenDTO(entity.Token, DateTime.Now),
                    RefrehTokenDTO = new DadosDoRefrehTokenDTO(entity.RefreshToken, entity.RefreshTokenExpireTime)
                },
                PerfisDeAcesso = []
            };

            if (entity.UsuarioCargoDepartamentos != null)
            {
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    usuario.CargoCodigo = item.CargoCodigo;
                    usuario.Cargo = await MapChildAsync(item.Cargo, _cargoMap);

                    usuario.DepartamentoCodigo = item.DepartamentoCodigo;
                    usuario.Departamento = await MapChildAsync(item.Departamento, _departamentoMap);
                }
            }

            if (entity.PerfisDeAcessoUsuario != null)
            {
                foreach (var item in entity.PerfisDeAcessoUsuario)
                {
                    var perfilDeAcesso = await MapChildAsync(item.PerfilDeAcesso, _perfilDeAcessoMap);

                    usuario.PerfisDeAcesso.Add(perfilDeAcesso);
                }
            }

            return usuario;
        }

        public override Task<UsuarioIdentity> MapToEntityAsync(UsuarioDTO dto)
        {
            return Task.FromResult(new UsuarioIdentity()
            {
                Codigo = dto.Codigo.ToUpper(),
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email,
                DataNascimento = dto.DataNascimento,
                SalarioAtual = dto.Salario
            });
        }
    }
}