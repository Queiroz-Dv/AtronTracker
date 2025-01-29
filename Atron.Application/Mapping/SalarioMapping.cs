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
            var dto = new SalarioDTO(entity.Id, entity.UsuarioId, entity.UsuarioCodigo, entity.SalarioMensal, entity.Ano, entity.MesId);


            var usuarioRelacionado = entity.Usuario.UsuarioCargoDepartamentos.First();

            dto.Usuario = new UsuarioDTO()
            {
                Codigo = usuarioRelacionado.UsuarioCodigo,
                Nome = usuarioRelacionado.Usuario.Nome,
                Sobrenome = usuarioRelacionado.Usuario.Sobrenome,
                Cargo = new CargoDTO()
                {
                    Codigo = usuarioRelacionado.Cargo.Codigo,
                    Descricao = usuarioRelacionado.Cargo.Descricao,
                },
                Departamento = new DepartamentoDTO()
                {
                    Codigo = usuarioRelacionado.Departamento.Codigo,
                    Descricao = usuarioRelacionado.Departamento.Descricao
                }
            };
            return dto;
        }

        public override Salario MapToEntity(SalarioDTO dto)
        {
            var usuarioBdTask = _usuarioRepository.ObterPorCodigoRepositoryAsync(dto.UsuarioCodigo);
            usuarioBdTask.Wait();
            var usuario = usuarioBdTask.Result;

            return new Salario(usuario.Id, usuario.Codigo, dto.SalarioMensal, dto.Ano, dto.MesId);
        }
    }
}