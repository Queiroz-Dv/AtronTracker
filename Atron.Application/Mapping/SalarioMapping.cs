using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class SalarioMapping : AsyncApplicationMapService<SalarioDTO, Salario>
    {
        private readonly IUsuarioService usuarioService;
        private readonly ISalarioRepository salarioRepository;

        public SalarioMapping(IUsuarioService usuarioService, ISalarioRepository salarioRepository)
        {
            this.usuarioService = usuarioService;
            this.salarioRepository = salarioRepository;
        }

        public override async Task<SalarioDTO> MapToDTOAsync(Salario entity)
        {
            var dto = new SalarioDTO()
            {
                Id = entity.Id,
                UsuarioCodigo = entity.UsuarioCodigo,
                SalarioMensal = entity.SalarioMensal,
                Ano = entity.Ano,
                MesId = entity.MesId
            };

            dto.Mes = new MesDTO()
            {
                Id = entity.MesId,
                Descricao = MesDTO.Meses().FirstOrDefault(ms => ms.Id == entity.MesId).Descricao
            };

            var usuario = await usuarioService.ObterPorCodigoAsync(entity.UsuarioCodigo);
            
            if (usuario != null)
            {
                dto.Usuario = usuario;
            }

            return dto;
        }

        public override async Task<Salario> MapToEntityAsync(SalarioDTO dto)
        {
            var usuario = await usuarioService.ObterPorCodigoAsync(dto.UsuarioCodigo);
            
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