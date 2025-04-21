using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class SalarioMapping : ApplicationMapService<SalarioDTO, Salario>
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public SalarioMapping(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public override SalarioDTO MapToDTO(Salario entity)
        {
            var dto = new SalarioDTO()
            {
                Id = entity.Id,
                UsuarioCodigo = entity.UsuarioCodigo,
                SalarioMensal = entity.SalarioMensal,
                Ano = entity.Ano,
                MesId = entity.MesId,

                Mes = new MesDTO()
                {
                    Id = entity.MesId,
                    Descricao = MesDTO.Meses().FirstOrDefault(ms => ms.Id == entity.MesId).Descricao
                }
            };

            if(entity.Usuario != null)
            {
                dto.Usuario = new UsuarioDTO()
                {
                    Codigo = entity.UsuarioCodigo,
                    Nome = entity.Usuario.Nome,
                    Sobrenome = entity.Usuario.Sobrenome,
                };


                if (entity.Usuario.UsuarioCargoDepartamentos.Any())
                {
                    var relacionamento = entity.Usuario.UsuarioCargoDepartamentos.First();

                    dto.Usuario.Cargo = new CargoDTO()
                    {
                        Codigo = relacionamento.Cargo.Codigo,
                        Descricao = relacionamento.Cargo.Descricao
                    };

                    dto.Usuario.Departamento = new DepartamentoDTO()
                    {
                        Codigo = relacionamento.Departamento.Codigo,
                        Descricao = relacionamento.Departamento.Descricao
                    };
                }                    
            }
            return dto;
        }

        public override Salario MapToEntity(SalarioDTO dto)
        {
            var usuarioBdTask = _usuarioRepository.ObterPorCodigoRepositoryAsync(dto.UsuarioCodigo);
            usuarioBdTask.Wait();
            var usuario = usuarioBdTask.Result;

            return new Salario()
            {
                Id = dto.Id,
                UsuarioId = usuario.Id,
                UsuarioCodigo = dto.UsuarioCodigo.ToUpper(),
                SalarioMensal = dto.SalarioMensal,
                Ano = dto.Ano,
                MesId = dto.MesId
            };
        }
    }
}