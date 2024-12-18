﻿using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using AutoMapper;
using Notification.Models;
using Shared.Models;
using System.Collections.Generic;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Tarefa> _repository;
        private readonly IRepository<TarefaEstado> _repositoryTarefaEstado;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly MessageModel<Tarefa> _messageModel;

        public TarefaService(IMapper mapper,
                             IRepository<Tarefa> repository,
                             IRepository<TarefaEstado> repositoryTarefaEstado,                            
                             IUsuarioRepository usuarioRepository,
                             IUsuarioService usuarioService,
                             ITarefaRepository tarefaRepository,
                             MessageModel<Tarefa> messageModel)
        {
            _mapper = mapper;
            _repository = repository;
            _repositoryTarefaEstado = repositoryTarefaEstado;
            _messageModel = messageModel;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
        }

        public async Task CriarAsync(TarefaDTO tarefaDTO)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo);
            if (usuario is not null)
            {
                tarefaDTO.UsuarioId = usuario.Id;
            }

            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);
            _messageModel.Validate(tarefa);

            if (!_messageModel.Messages.HasErrors())
            {
                await _tarefaRepository.CriarRepositoryAsync(tarefa);
                _messageModel.AddSuccessMessage(nameof(Tarefa));                
            }
        }

        public async Task<List<TarefaDTO>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefasComEstado();
            var estadosDaTarefa = await _repositoryTarefaEstado.ObterTodosRepositoryAsync();
            var usuarios = await _usuarioService.ObterTodosAsync();

            var tarefasDTO = _mapper.Map<IEnumerable<TarefaDTO>>(tarefas);
            var tarefasDTOList = new List<TarefaDTO>();

            foreach (var tarefaDTO in tarefasDTO)
            {
                foreach (var usuario in usuarios)
                {
                    if (tarefaDTO.UsuarioId == usuario.Id)
                    {
                        var tarefa = new TarefaDTO();
                        tarefa.UsuarioCodigo = tarefaDTO.UsuarioCodigo;

                        tarefa.Usuario = new UsuarioDTO()
                        {
                            Codigo = usuario.Codigo,
                            Nome = usuario.Nome,
                            Sobrenome = usuario.Sobrenome,
                            DataNascimento = usuario.DataNascimento,
                            Salario = usuario.Salario,
                            CargoCodigo = usuario.CargoCodigo,
                            DepartamentoCodigo = usuario.DepartamentoCodigo,

                            Departamento = new DepartamentoDTO()
                            {
                                Codigo = usuario.Departamento.Codigo,
                                Descricao = usuario.Departamento.Descricao,
                            },

                            Cargo = new CargoDTO()
                            {
                                Codigo = usuario.Cargo.Codigo,
                                Descricao = usuario.Cargo.Descricao,
                            }
                        };

                        tarefa.Titulo = tarefaDTO.Titulo;
                        tarefa.Conteudo = tarefaDTO.Conteudo;
                        tarefa.DataInicial = tarefaDTO.DataInicial.Date;
                        tarefa.DataFinal = tarefaDTO.DataFinal.Date;
                        tarefa.EstadoDaTarefa = tarefaDTO.EstadoDaTarefa;
                        tarefa.EstadoDaTarefaDescricao = estadosDaTarefa.FirstOrDefault(tre => tre.Id == tarefa.EstadoDaTarefa).Descricao;

                        tarefasDTOList.Add(tarefa);
                    }
                }
            }

            return tarefasDTOList;
        }

        public async Task AtualizarAsync(TarefaDTO tarefaDTO)
        {
            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);

            var usuarioExiste = _usuarioRepository.UsuarioExiste(tarefa.UsuarioCodigo);

            if (usuarioExiste)
            {
                var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefa.UsuarioCodigo);
                //var tarefaEstados = await _tarefaEstadoRepository.ObterTodosAsync();

                tarefa.UsuarioId = usuario.Id;
                tarefa.UsuarioCodigo = usuario.Codigo;
                // tarefa.EstadoDaTarefa = tarefaEstados.FirstOrDefault(tre => tre.Id == tarefa.EstadoDaTarefa).Id;

                _notification.Validate(tarefa);

                if (!_notification.Messages.HasErrors())
                {
                    await _tarefaRepository.AtualizarRepositoryAsync(tarefa);
                    //Messages.Add(new NotificationMessage("Tarefa atualizada com sucesso."));
                    return;
                }

                //Messages.AddRange(_notification.Messages);
            }
            else
            {
                //Messages.Add(new NotificationMessage("Código de usuário não existe. Tente novamente", Notification.Enums.ENotificationType.Error));
                return;
            }
        }

        public async Task ExcluirAsync(int id)
        {
            var tarefa = await _tarefaRepository.ObterPorIdRepositoryAsync(id);

            if (tarefa is null)
            {
                // Messages.Add(new NotificationMessage("Tarefa não existe. Tente novamente"));
                return;
            }

            await _repository.RemoverRepositoryAsync(tarefa);
            // Messages.Add(new NotificationMessage("Registro removido com sucesso"));
        }
    }
}