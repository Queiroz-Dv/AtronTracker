﻿using Atron.Application.DTO;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Services.Mapper;
using System.Linq;

namespace Atron.Application.Mapping
{
    public class TarefaMapping : ApplicationMapService<TarefaDTO, Tarefa>
    {
        private readonly IRepository<TarefaEstado> _repositoryTarefaEstado;
        private readonly ICargoRepository cargoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public TarefaMapping(IRepository<TarefaEstado> repository, IUsuarioRepository usuarioRepository, ICargoRepository cargoRepository)
        {
            _repositoryTarefaEstado = repository;
            _usuarioRepository = usuarioRepository;
            this.cargoRepository = cargoRepository;
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

            var usuarioRelacionado = entity.Usuario.UsuarioCargoDepartamentos.First();

            dto.Usuario = new UsuarioDTO()
            {
                Codigo = usuarioRelacionado.UsuarioCodigo,
                Nome = usuarioRelacionado.Usuario.Nome,
                Cargo = new CargoDTO()
                {
                    Codigo = usuarioRelacionado.Cargo.Codigo,
                    Descricao = usuarioRelacionado.Cargo.Descricao,
                },
                Departamento = new DepartamentoDTO()
                {
                    Codigo = usuarioRelacionado.Departamento.Codigo,
                    Descricao = usuarioRelacionado.Departamento.Descricao
                }
            };


            return dto;
        }

        public override Tarefa MapToEntity(TarefaDTO dto)
        {
            var usuarioBdTask = _usuarioRepository.ObterUsuarioPorCodigoAsync(dto.UsuarioCodigo);
            usuarioBdTask.Wait();
            var usuario = usuarioBdTask.Result;

            return new Tarefa(usuario.Id,
                              usuario.Codigo,
                              dto.Titulo,
                              dto.Conteudo,
                              dto.DataInicial,
                              dto.DataFinal,
                              dto.TarefaEstadoId);
        }
    }
}
