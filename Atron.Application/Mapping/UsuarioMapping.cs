using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.DTO.API;
using Shared.Services.Mapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class UsuarioMapping : AsyncApplicationMapService<UsuarioDTO, UsuarioIdentity>
    {
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;

        public UsuarioMapping(IDepartamentoRepository departamentoRepository, ICargoRepository cargoRepository, IPerfilDeAcessoRepository perfilDeAcessoRepository)
        {
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
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
                }
            };

            if (entity.UsuarioCargoDepartamentos != null)
            {
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(item.CargoCodigo);
                    var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(item.DepartamentoCodigo);

                    usuario.CargoCodigo = cargo.Codigo;
                    usuario.Cargo = new CargoDTO(cargo.Codigo, cargo.Descricao);

                    usuario.DepartamentoCodigo = departamento.Codigo;
                    usuario.Departamento = new DepartamentoDTO(departamento.Codigo, departamento.Descricao);
                }
            }

            if(entity.PerfisDeAcessoUsuario != null)
            {
                usuario.PerfisDeAcesso = new List<PerfilDeAcessoDTO>();

                foreach (var item in entity.PerfisDeAcessoUsuario)
                {
                    var perfilDeAcesso= await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(item.PerfilDeAcessoCodigo);
                 
                    usuario.PerfisDeAcesso.Add(new PerfilDeAcessoDTO
                    {
                        Codigo = perfilDeAcesso.Codigo,
                        Descricao = perfilDeAcesso.Descricao
                    });
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