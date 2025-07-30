using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Services.Mapper;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : ApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IUsuarioService usuarioService;
        private readonly ITarefaRepository _tarefaRepository;

        public TarefaMapping(IUsuarioService usuarioService, ITarefaRepository tarefaRepository)
        {
            this.usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
        }

        public override TarefaDTO MapToDTO(Tarefa entity)
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

            var usuarioTask = usuarioService.ObterPorCodigoAsync(entity.UsuarioCodigo);
            usuarioTask.Wait();
            var usuario = usuarioTask.Result;
            if (usuario != null)
            {
                dto.Usuario = usuario;
            }
                    
            if (entity.TarefaEstadoId != 0)
            {
                var tarefaEstadoDescricaoTask = _tarefaRepository.ObterDescricaoTarefaEstado(entity.TarefaEstadoId);
                tarefaEstadoDescricaoTask.Wait();
                var tarefaEstadoDescricao = tarefaEstadoDescricaoTask.Result;
                dto.EstadoDaTarefa = new TarefaEstado() { Id = entity.TarefaEstadoId, Descricao = tarefaEstadoDescricao };
            }

            return dto;
        }

        public override Tarefa MapToEntity(TarefaDTO dto)
        {
            var usuarioBdTask = usuarioService.ObterPorCodigoAsync(dto.UsuarioCodigo);
            usuarioBdTask.Wait();
            var usuario = usuarioBdTask.Result;

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
