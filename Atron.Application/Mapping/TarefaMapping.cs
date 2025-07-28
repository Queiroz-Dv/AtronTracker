using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Services.Mapper;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : ApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITarefaRepository _tarefaRepository;

        public TarefaMapping(IUsuarioRepository usuarioRepository, ITarefaRepository tarefaRepository)
        {
            _usuarioRepository = usuarioRepository;
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
                Usuario = new UsuarioDTO()
                {
                    Codigo = entity.UsuarioCodigo,
                    Nome = entity.Usuario.Nome,
                    Sobrenome = entity.Usuario.Sobrenome
                }
            };

            if (entity.Usuario.UsuarioCargoDepartamentos != null)
            {
                foreach (var item in entity.Usuario.UsuarioCargoDepartamentos)
                {
                    dto.Usuario.Cargo = new CargoDTO { Codigo = item.Cargo.Codigo, Descricao = item.Cargo.Descricao };

                    dto.Usuario.Departamento = new DepartamentoDTO(item.Departamento.Codigo, item.Departamento.Descricao);
                }
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
            var usuarioBdTask = _usuarioRepository.ObterUsuarioPorCodigoAsync(dto.UsuarioCodigo);
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
