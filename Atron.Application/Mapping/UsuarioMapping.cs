using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.DTO.API;
using Shared.Services.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class UsuarioMapping : ApplicationMapService<UsuarioDTO, UsuarioIdentity>
    {
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;

        public UsuarioMapping(IDepartamentoRepository departamentoRepository, ICargoRepository cargoRepository)
        {
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
        }

        public override UsuarioDTO MapToDTO(UsuarioIdentity entity)
        {
            var usuario = new UsuarioDTO
            {
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

            if (entity.UsuarioCargoDepartamentos.Any())
            {
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    var cargoBdTask = _cargoRepository.ObterCargoPorCodigoAsyncAsNoTracking(item.CargoCodigo);
                    cargoBdTask.Wait();
                    var cargoBd = cargoBdTask.Result;

                    var departamentoBdTask = _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(item.DepartamentoCodigo);
                    departamentoBdTask.Wait();
                    var departamentoBd = departamentoBdTask.Result;

                    usuario.Cargo = new CargoDTO
                    {
                        Codigo = cargoBd.Codigo,
                        Descricao = cargoBd.Descricao
                    };

                    usuario.Departamento = new DepartamentoDTO(departamentoBd.Codigo, departamentoBd.Descricao);
                        
                    usuario.CargoCodigo = cargoBd.Codigo;
                    usuario.DepartamentoCodigo = departamentoBd.Codigo;
                }

                return usuario;
            }

            return usuario;
        }

        public override UsuarioIdentity MapToEntity(UsuarioDTO dto)
        {
            // 1. Mapear o DTO para a entidade
            return new UsuarioIdentity()
            {
                Codigo = dto.Codigo.ToUpper(),
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email,
                DataNascimento = dto.DataNascimento,
                SalarioAtual = dto.Salario
            };
        }
    }
}