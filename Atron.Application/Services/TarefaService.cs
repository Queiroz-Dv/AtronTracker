using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using AutoMapper;
using Shared.Extensions;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Collections.Generic;
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
        private readonly IValidateModel<Tarefa> _validateModel;
        private readonly MessageModel _messageModel;

        public TarefaService(IMapper mapper,
                             IRepository<Tarefa> repository,
                             IRepository<TarefaEstado> repositoryTarefaEstado,
                             IUsuarioRepository usuarioRepository,
                             IUsuarioService usuarioService,
                             ITarefaRepository tarefaRepository,
                             MessageModel messageModel,
                             IValidateModel<Tarefa> validateModel)
        {
            _mapper = mapper;
            _repository = repository;
            _repositoryTarefaEstado = repositoryTarefaEstado;
            _messageModel = messageModel;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _tarefaRepository = tarefaRepository;
            _validateModel = validateModel;
        }

        public async Task CriarAsync(TarefaDTO tarefaDTO)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefaDTO.UsuarioCodigo);
            if (usuario is not null)
            {
                tarefaDTO.UsuarioId = usuario.Id;
            }

            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);

            var tarefaEstado = await _repositoryTarefaEstado.ObterPorIdRepositoryAsync(tarefaDTO.TarefaEstadoId);

            if (tarefaEstado is not null)
            {
                tarefa.TarefaEstadoId = tarefaEstado.Id;
            }

            _validateModel.Validate(tarefa);

            if (!_messageModel.Messages.HasErrors())
            {
                await _tarefaRepository.CriarTarefaAsync(tarefa);
                _messageModel.AddSuccessMessage(nameof(Tarefa));
            }
        }

        public async Task<List<TarefaDTO>> ObterTodosAsync()
        {
            var tarefas = await _tarefaRepository.ObterTodasTarefas();
            var tarefaEstado = await _repositoryTarefaEstado.ObterTodosRepositoryAsync();

            var tarefasDTO = _mapper.Map<IEnumerable<TarefaDTO>>(tarefas);

            var entidades = (from trf in tarefasDTO
                             join tre in tarefaEstado on trf.TarefaEstadoId equals tre.Id
                             select new TarefaDTO
                             {
                                 Id = trf.Id,
                                 Titulo = trf.Titulo,
                                 Conteudo = trf.Conteudo,
                                 DataInicial = trf.DataInicial,
                                 DataFinal = trf.DataFinal,
                                 TarefaEstadoId = tre.Id,
                                 EstadoDaTarefaDescricao = tre.Descricao,
                                 UsuarioCodigo = trf.UsuarioCodigo,

                                 Usuario = new UsuarioDTO()
                                 {
                                     Codigo = trf.Usuario.Codigo,
                                     Nome = trf.Usuario.Nome,
                                     Sobrenome = trf.Usuario.Sobrenome,
                                     DataNascimento = trf.Usuario.DataNascimento,
                                     Salario = trf.Usuario.Salario,
                                     CargoCodigo = trf.Usuario.CargoCodigo,
                                     DepartamentoCodigo = trf.Usuario.DepartamentoCodigo,

                                     Departamento = new DepartamentoDTO()
                                     {
                                         Codigo = trf.Usuario.Departamento.Codigo,
                                         Descricao = trf.Usuario.Departamento.Descricao
                                     },

                                     Cargo = new CargoDTO()
                                     {
                                         Codigo = trf.Usuario.Cargo.Codigo,
                                         Descricao = trf.Usuario.Cargo.Descricao
                                     }
                                 }
                             }).ToList();

            return entidades;
        }

        public async Task AtualizarAsync(TarefaDTO tarefaDTO)
        {
            if (tarefaDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Tarefa));
                return;
            }

            var tarefa = _mapper.Map<Tarefa>(tarefaDTO);

            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(tarefa.UsuarioCodigo);
            var tarefaEstado = await _repositoryTarefaEstado.ObterPorIdRepositoryAsync(tarefaDTO.TarefaEstadoId);

            tarefa.UsuarioId = usuario.Id;
            tarefa.UsuarioCodigo = usuario.Codigo;
            tarefa.TarefaEstadoId = tarefaEstado.Id;

            _validateModel.Validate(tarefa);

            if (!_messageModel.Messages.HasErrors())
            {
                await _tarefaRepository.AtualizarRepositoryAsync(tarefa);
                _messageModel.AddUpdateMessage(nameof(Tarefa));
                return;
            }
        }

        public async Task ExcluirAsync(string id)
        {
            var tarefa = await _tarefaRepository.ObterPorIdRepositoryAsync(Convert.ToInt32(id));

            if (tarefa is null)
            {
                _messageModel.AddRegisterNotFoundMessage(nameof(Tarefa));
            }
            else
            {
                await _repository.RemoverRepositoryAsync(tarefa);
                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Tarefa));
            }
        }

        public async Task<TarefaDTO> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);
            var dto = _mapper.Map<TarefaDTO>(tarefaRepository);

            var tarefaDTO = new TarefaDTO
            {
                Id = dto.Id,
                Titulo = dto.Titulo,
                Conteudo = dto.Conteudo,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal,
                TarefaEstadoId = dto.TarefaEstadoId,
                EstadoDaTarefaDescricao = dto.EstadoDaTarefaDescricao,
                UsuarioCodigo = dto.UsuarioCodigo,

                Usuario = new UsuarioDTO()
                {
                    Codigo = dto.Usuario.Codigo,
                    Nome = dto.Usuario.Nome,
                    Sobrenome = dto.Usuario.Sobrenome,
                    DataNascimento = dto.Usuario.DataNascimento,
                    Salario = dto.Usuario.Salario,
                    CargoCodigo = dto.Usuario.CargoCodigo,
                    DepartamentoCodigo = dto.Usuario.DepartamentoCodigo,

                    Departamento = new DepartamentoDTO()
                    {
                        Codigo = dto.Usuario.Departamento.Codigo,
                        Descricao = dto.Usuario.Departamento.Descricao
                    },

                    Cargo = new CargoDTO()
                    {
                        Codigo = dto.Usuario.Cargo.Codigo,
                        Descricao = dto.Usuario.Cargo.Descricao
                    }
                }
            };

            return tarefaDTO;
        }
    }
}