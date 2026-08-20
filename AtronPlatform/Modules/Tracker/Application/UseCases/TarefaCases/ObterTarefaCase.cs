using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterTarefaCase(
        IToDtoMapper<Tarefa, TarefaDTO> mapper,
        ITarefaRepository tarefaRepository)
    {
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _mapper = mapper;
        private readonly ITarefaRepository _repository = tarefaRepository;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(int id)
        {
            var tarefa = await _repository.ObterTarefaPorId(id);
            return tarefa is null
                ? Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<TarefaDTO>.Sucesso(_mapper.MapToDto(tarefa));
        }

        public async Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ObterTodosAsync()
        {
            var entidades = await _repository.ObterTodasTarefas();
            var dtos = _mapper.MapToDtos(entidades);
            var resultado = Resultado<IReadOnlyCollection<TarefaDTO>>.Sucesso(dtos.ToList());
            return resultado;
        }

    }
}