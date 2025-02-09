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


            var usuario = entity.Usuario.UsuarioCargoDepartamentos.First();

            dto.Usuario = new UsuarioDTO()
            {
                Codigo = usuario.UsuarioCodigo,
                Nome = usuario.Usuario.Nome,
                Sobrenome = usuario.Usuario.Sobrenome,
                Cargo = new CargoDTO()
                {
                    Codigo = usuario.Cargo.Codigo,
                    Descricao = usuario.Cargo.Descricao,
                },
                Departamento = new DepartamentoDTO(usuario.Departamento.Codigo, usuario.Departamento.Descricao)             
            };
            return dto;
        }

        public override Salario MapToEntity(SalarioDTO dto)
        {
            var usuarioBdTask = _usuarioRepository.ObterPorCodigoRepositoryAsync(dto.UsuarioCodigo);
            usuarioBdTask.Wait();
            var usuario = usuarioBdTask.Result;

            return new Salario()
            {
                UsuarioId = usuario.Id,
                UsuarioCodigo = dto.UsuarioCodigo.ToUpper(),
                SalarioMensal = dto.SalarioMensal,
                Ano = dto.Ano,
                MesId = dto.MesId
            };
        }
    }
}