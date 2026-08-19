using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaEstadoService(ITarefaEstadoRepository tarefaEstadoRepository,
        IToDtoMapper<TarefaEstado, TarefaEstadoDTO> mapper)
    {
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository = tarefaEstadoRepository;
        private readonly IToDtoMapper<TarefaEstado, TarefaEstadoDTO> _mapper = mapper;

        public async Task<Resultado<List<TarefaEstadoDTO>>> ObterTodosAsync()
        {
            var estados = await _tarefaEstadoRepository.ObterTodosAsync();
            return Resultado<List<TarefaEstadoDTO>>.Sucesso(_mapper.MapToDtos(estados).ToList());
        }
    }
}
