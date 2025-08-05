using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Services.Mapper;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : AsyncApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IUsuarioService usuarioService;

        public TarefaMapping(IUsuarioService usuarioService)
        {
            this.usuarioService = usuarioService;
        }

        public override async  Task<TarefaDTO>  MapToDTOAsync(Tarefa entity)
        {
            var dto = new TarefaDTO
            {
                Id = entity.Id,
                Titulo = entity.Titulo,
                Conteudo = entity.Conteudo,
                DataInicial = entity.DataInicial,
                DataFinal = entity.DataFinal,
                UsuarioCodigo = entity.UsuarioCodigo,              
            };

            var usuario = await usuarioService.ObterPorCodigoAsync(entity.UsuarioCodigo);

            if (usuario != null)
            {
                dto.Usuario = usuario;
            }

            if (!entity.TarefaEstadoId.ToString().IsNullOrEmpty())
            {
                dto.EstadoDaTarefa = new TarefaEstadoDTO() { Id = entity.TarefaEstadoId, Descricao = TarefaEstadoDTO.TarefasEstados().FirstOrDefault(trf => trf.Id == entity.TarefaEstadoId).Descricao };
            }

            return dto;
        }

        public override async Task<Tarefa> MapToEntityAsync(TarefaDTO dto)
        {
            var usuario = await usuarioService.ObterPorCodigoAsync(dto.UsuarioCodigo);
          
            return new Tarefa
            {
                UsuarioId = usuario.Id,
                UsuarioCodigo = usuario.Codigo,
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.EstadoDaTarefa?.Id ?? 0,
            };
        }
    }
}
