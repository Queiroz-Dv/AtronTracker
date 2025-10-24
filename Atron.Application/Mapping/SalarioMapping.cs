using Atron.Application.DTO;
using Atron.Tracker.Domain.Entities;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class SalarioMapping : AsyncApplicationMapService<SalarioDTO, Salario>
    {
        private readonly IAsyncApplicationMapService<UsuarioDTO, Usuario> _usuarioMap;

        public SalarioMapping(IAsyncApplicationMapService<UsuarioDTO, Usuario> usuarioMap) : base()
        {
            _usuarioMap = usuarioMap;
        }

        public override async Task<SalarioDTO> MapToDTOAsync(Salario entity)
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
                },

                Usuario = await MapChildAsync(entity.Usuario, _usuarioMap)
            };

            return dto;
        }

        public override Task<Salario> MapToEntityAsync(SalarioDTO dto)
        {
            return Task.FromResult(new Salario()
            {
                Id = dto.Id,
                UsuarioCodigo = dto.UsuarioCodigo.ToUpper(),
                SalarioMensal = dto.SalarioMensal,
                Ano = dto.Ano,
                MesId = dto.MesId
            });
        }
    }
}