using Atron.Application.DTO;
using Atron.Application.Interfaces.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Shared.Extensions;
using Shared.Interfaces.Mapper;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Services.EntitiesServices
{
    public class TarefaService : ITarefaService
    {
        private readonly IApplicationMapService<TarefaDTO, Tarefa> _map;
        private readonly ITarefaRepository _tarefaRepository;
        private readonly IValidateModel<Tarefa> _validateModel;
        private readonly MessageModel _messageModel;

        public TarefaService(IApplicationMapService<TarefaDTO, Tarefa> map,
                             ITarefaRepository tarefaRepository,
                             MessageModel messageModel,
                             IValidateModel<Tarefa> validateModel)
        {
            _map = map;
            _messageModel = messageModel;
            _tarefaRepository = tarefaRepository;
            _validateModel = validateModel;
        }

        public async Task CriarAsync(TarefaDTO tarefaDTO)
        {
            var tarefa = _map.MapToEntity(tarefaDTO);
            _validateModel.Validate(tarefa);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                await _tarefaRepository.CriarTarefaAsync(tarefa);
                _messageModel.AddMessage("Tarefa gravada com sucesso.");
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
                _messageModel.AddRegisterInvalidMessage();
                return;
            }

            var tarefa = _map.MapToEntity(tarefaDTO);
            _validateModel.Validate(tarefa);

            if (!_messageModel.Notificacoes.HasErrors())
            {
                await _tarefaRepository.AtualizarTarefaAsync(id, tarefa);
                _messageModel.AddMessage("Tarefa atualizada com sucesso");
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
                _messageModel.AddMessage("Tarefa removida com sucesso");
            }
        }

        public async Task<TarefaDTO> ObterPorId(int id)
        {
            var tarefaRepository = await _tarefaRepository.ObterTarefaPorId(id);

            if (tarefaRepository is null)
            {
                _messageModel.AddRegisterNotFoundMessage();
                return null;
            }

            return _map.MapToDTO(tarefaRepository);            
        }
    }
}