using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterTarefasDisponiveisCase(
        ITarefaRepository tarefaRepository,
        IToDtoMapper<Tarefa, TarefaDTO> mapper)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<TarefaDTO>>> ExecutarAsync()
        {
            var tarefas = await _tarefaRepository.ObterTarefasAtivasDisponiveisAsync();
            var dtos = _mapper.MapToDtos(tarefas).ToList();

            return Resultado<IReadOnlyCollection<TarefaDTO>>.Sucesso(dtos);
        }
    }
}
