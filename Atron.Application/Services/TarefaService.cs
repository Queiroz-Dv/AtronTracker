using Atron.Application.DTO;
using Atron.Application.Interfaces;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly IApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly IRepository<TarefaEstado> _repositoryTarefaEstado;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IValidateModel<Tarefa> _validateModel;
        private readonly MessageModel _messageModel;

        public TarefaService(IApplicationMapService<TarefaDTO, Tarefa> map,
                             IRepository<TarefaEstado> repositoryTarefaEstado,
                             ITarefaRepository tarefaRepository,
                             MessageModel messageModel,
                             IValidateModel<Tarefa> validateModel)
        {
            _map = map;
            _repositoryTarefaEstado = repositoryTarefaEstado;
            _messageModel = messageModel;
            _tarefaRepository = tarefaRepository;
            _validateModel = validateModel;
        }

        public async Task CriarAsync(TarefaDTO tarefaDTO)
        {
            var tarefa = _map.MapToEntity(tarefaDTO);
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
            return _map.MapToListDTO(tarefas);            
        }

        public async Task AtualizarAsync(int id, TarefaDTO tarefaDTO)
        {
            if (tarefaDTO is null)
            {
                _messageModel.AddRegisterInvalidMessage(nameof(Tarefa));
                return;
            }

            var tarefa = _map.MapToEntity(tarefaDTO);
            _validateModel.Validate(tarefa);

            if (!_messageModel.Messages.HasErrors())
            {
                await _tarefaRepository.AtualizarTarefaAsync(id, tarefa);
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
                await _tarefaRepository.RemoverRepositoryAsync(tarefa);
                _messageModel.AddRegisterRemovedSuccessMessage(nameof(Tarefa));
            }
        }

        public async Task<TarefaDTO> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);
            return _map.MapToDTO(tarefaRepository);            
        }
    }
}