using Atron.Application.DTO;
using Atron.Tracker.Domain.Entities;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : AsyncApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IAsyncApplicationMapService<UsuarioDTO, Usuario> _usuarioMap;

        public TarefaMapping(IAsyncApplicationMapService<UsuarioDTO, Usuario> usuarioMap) : base()
        {
            _usuarioMap = usuarioMap;
        }

        public override async Task<TarefaDTO> MapToDTOAsync(Tarefa entity)
        {
            var dto = new TarefaDTO
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Conteudo = entity.Conteudo,
                DataInicial = entity.DataInicial,
                DataFinal = entity.DataFinal,
                UsuarioCodigo = entity.UsuarioCodigo,
                Usuario = await MapChildAsync(entity.Usuario, _usuarioMap)
            };

            if (!entity.TarefaEstadoId.ToString().IsNullOrEmpty())
            {
                dto.EstadoDaTarefa = new TarefaEstadoDTO() { Id = entity.TarefaEstadoId, Descricao = TarefaEstadoDTO.TarefasEstados().FirstOrDefault(trf => trf.Id == entity.TarefaEstadoId).Descricao };
            }

            return dto;
        }

        public override Task<Tarefa> MapToEntityAsync(TarefaDTO dto)
        {
            return Task.FromResult(new Tarefa
            {
                UsuarioCodigo = dto.UsuarioCodigo,
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0,
            });
        }
    }
}
