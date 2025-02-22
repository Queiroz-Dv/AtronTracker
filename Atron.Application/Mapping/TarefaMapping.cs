using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Services.Mapper;
using System;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : ApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IRepository<Usuario> _usuarioRepository;

        public TarefaMapping(IRepository<Usuario> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
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
                UsuarioCodigo = entity.UsuarioCodigo
            };

            if (!entity.TarefaEstadoId.ToString().IsNullOrEmpty())
            {
                dto.EstadoDaTarefaId = entity.TarefaEstadoId.ToString();
            }

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
                TarefaEstadoId = Convert.ToInt16(dto.EstadoDaTarefaId)
            };
        }
    }
}
