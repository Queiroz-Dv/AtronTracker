using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Services.Mapper;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : ApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IRepository<TarefaEstado> _repositoryTarefaEstado;
        private readonly IRepository<Usuario> _usuarioRepository;

        public TarefaMapping(IRepository<TarefaEstado> repository, IRepository<Usuario> usuarioRepository)
        {
            _repositoryTarefaEstado = repository;
            _usuarioRepository = usuarioRepository;
        }

        public override TarefaDTO MapToDTO(Tarefa entity)
        {
            var dto = new TarefaDTO(entity.Id,
                                    entity.Titulo,
                                    entity.Conteudo,
                                    entity.DataInicial,
                                    entity.DataFinal,
                                    entity.UsuarioCodigo);


            if (!entity.TarefaEstadoId.ToString().IsNullOrEmpty())
            {
                var tarefaEstadoBdTask = _repositoryTarefaEstado.ObterPorIdRepositoryAsync(entity.TarefaEstadoId);
                tarefaEstadoBdTask.Wait();
                var tarefaEstado = tarefaEstadoBdTask.Result;

                dto.TarefaEstadoId = tarefaEstado.Id;
                dto.EstadoDaTarefaDescricao = tarefaEstado.Descricao;
            }

            var usuario = entity.Usuario.UsuarioCargoDepartamentos.First();

            dto.Usuario = new UsuarioDTO()
            {
                Codigo = usuario.UsuarioCodigo,
                Nome = usuario.Usuario.Nome,
                Cargo = new CargoDTO()
                {
                    Codigo = usuario.Cargo.Codigo,
                    Descricao = usuario.Cargo.Descricao,
                },
                Departamento = new DepartamentoDTO(usuario.Departamento.Codigo, usuario.Departamento.Descricao)            
            };


            return dto;
        }

        public override Tarefa MapToEntity(TarefaDTO dto)
        {
            var usuarioBdTask = _usuarioRepository.ObterPorCodigoRepositoryAsync(dto.UsuarioCodigo);
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
                TarefaEstadoId = dto.TarefaEstadoId
            };            
        }
    }
}
