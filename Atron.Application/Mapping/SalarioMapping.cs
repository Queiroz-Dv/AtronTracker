using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class SalarioMapping : ApplicationMapService<SalarioDTO, Salario>
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public SalarioMapping(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public override SalarioDTO MapToDTO(Salario entity) => new SalarioDTO
        {
            Id = entity.Id,
            UsuarioCodigo = entity.UsuarioCodigo,
            SalarioMensal = entity.SalarioMensal,
            Ano = entity.Ano,
            MesId = entity.MesId
        };

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