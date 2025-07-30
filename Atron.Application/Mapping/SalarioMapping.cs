using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;

namespace Atron.Application.Mapping
{
    public class SalarioMapping : ApplicationMapService<SalarioDTO, Salario>
    {
        private readonly IUsuarioService usuarioService;
        private readonly ISalarioRepository salarioRepository;

        public SalarioMapping(IUsuarioService usuarioService, ISalarioRepository salarioRepository)
        {
            this.usuarioService = usuarioService;
            this.salarioRepository = salarioRepository;
        }

        public override SalarioDTO MapToDTO(Salario entity)
        {
            var dto = new SalarioDTO()
            {
                Id = entity.Id,
                UsuarioCodigo = entity.UsuarioCodigo,
                SalarioMensal = entity.SalarioMensal,
                Ano = entity.Ano,
                MesId = entity.MesId
            };

            var mesTask = salarioRepository.ObterDescricaoDoMes(entity.MesId);
            mesTask.Wait();
            dto.Mes = new Mes()
            {
                Id = entity.MesId,
                Descricao = mesTask.Result
            };

            var usuarioTask = usuarioService.ObterPorCodigoAsync(entity.UsuarioCodigo);
            usuarioTask.Wait();
            var usuario = usuarioTask.Result;
            if (usuario != null)
            {
                dto.Usuario = usuario;
            }
            return dto;
        }

        public override Salario MapToEntity(SalarioDTO dto)
        {
            var usuarioBdTask = usuarioService.ObterPorCodigoAsync(dto.UsuarioCodigo);
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