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
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;

        public TarefaMapping(IRepository<Usuario> usuarioRepository, IDepartamentoRepository departamentoRepository, ICargoRepository cargoRepository)
        {
            _usuarioRepository = usuarioRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
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

            if (entity.Usuario.UsuarioCargoDepartamentos.Any())
            {
                foreach (var item in entity.Usuario.UsuarioCargoDepartamentos)
                {
                    dto.Usuario.Cargo = new CargoDTO { Codigo = item.Cargo.Codigo, Descricao = item.Cargo.Descricao };

                    dto.Usuario.Departamento = new DepartamentoDTO(item.Departamento.Codigo, item.Departamento.Descricao);
                }
            }

            if (!entity.TarefaEstadoId.ToString().IsNullOrEmpty())
            {
                dto.EstadoDaTarefa = new TarefaEstadoDTO() { Id =  entity.TarefaEstadoId, Descricao = TarefaEstadoDTO.TarefasEstados().FirstOrDefault(trf => trf.Id == entity.TarefaEstadoId).Descricao };                    
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
                TarefaEstadoId = Convert.ToInt16(dto.EstadoDaTarefa.Id)
            };
        }
    }
}
