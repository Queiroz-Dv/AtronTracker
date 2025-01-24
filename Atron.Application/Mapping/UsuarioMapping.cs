using Atron.Application.DTO;
using Atron.Domain.Entities;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class UsuarioMapping : ApplicationMapService<UsuarioDTO, Usuario>
    {
        private readonly IApplicationMapService<CargoDTO, Cargo> _cargoMapping;
        private readonly IApplicationMapService<DepartamentoDTO, Departamento> _departamentoMapping;

        public UsuarioMapping(
            IApplicationMapService<CargoDTO, Cargo> cargoMapping,
            IApplicationMapService<DepartamentoDTO, Departamento> departamentoMapping)
        {
            _cargoMapping = cargoMapping;
            _departamentoMapping = departamentoMapping;
        }

        public override UsuarioDTO MapToDTO(Usuario entity)
        {
            var usuario = new UsuarioDTO
            {
                Codigo = entity.Codigo,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                Email = entity.Email,
                Cargos = new List<CargoDTO>(),
                Departamentos = new List<DepartamentoDTO>(),
            };


            if (entity.UsuarioCargoDepartamentos != null || entity.UsuarioCargoDepartamentos.Count > 0)
            {                
                foreach (var item in entity.UsuarioCargoDepartamentos)
                {
                    var cargo = _cargoMapping.MapToDTO(item.Cargo);
                    var departamento = _departamentoMapping.MapToDTO(item.Cargo.Departamento);

                    usuario.Cargos.Add(cargo);
                    usuario.Departamentos.Add(departamento);
                }

                return usuario;
            }

            return usuario;
        }

        public override Usuario MapToEntity(UsuarioDTO dto)
        {
            var usuario = new Usuario
            {            
                Codigo = dto.Codigo,
                Nome = dto.Nome,
                Sobrenome = dto.Sobrenome,
                Email = dto.Email                
            };

            var cargo = _cargoMapping.MapToEntity(dto.Cargo);
            var departamento = _departamentoMapping.MapToEntity(dto.Departamento);

            usuario.UsuarioCargoDepartamentos = new List<UsuarioCargoDepartamento>
            {
                new UsuarioCargoDepartamento
                {
                    Cargo = cargo,
                    Departamento = departamento
                }
            };

            return usuario;
        }
    }
}
