using Application.DTO;
using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using System.Threading.Tasks;

namespace Application.Mapping
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

            if (entity.TarefaEstadoId > 0)
            {
                dto.EstadoDaTarefa = new TarefaEstadoDTO
                {
                    Id = entity.TarefaEstadoId,
                    Descricao = entity.EstadoDaTarefa?.Descricao
                };
            }

            return dto;
        }

        public override Task<Tarefa> MapToEntityAsync(TarefaDTO dto)
        {
            return Task.FromResult(new Tarefa
            {
                Id = dto.Id,
                UsuarioCodigo = dto.UsuarioCodigo?.ToUpper(),
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0,
            });
        }
    }
}
