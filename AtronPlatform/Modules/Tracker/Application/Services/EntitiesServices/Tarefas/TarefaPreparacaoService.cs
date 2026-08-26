using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaPreparacaoService(
        TarefaRelacionamentoService tarefaRelacionamentoService,
        ITarefaEstadoRepository tarefaEstadoRepository,
        IMapper<TarefaEstado, TarefaEstadoDTO> tarefaEstadoMap,
        IToEntityMapper<Tarefa, TarefaDTO> map,
        IValidador<TarefaDTO> validador) : ITarefaPreparacaoService
    {
        private readonly TarefaRelacionamentoService _tarefaRelacionamentoService = tarefaRelacionamentoService;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository = tarefaEstadoRepository;
        private readonly IMapper<TarefaEstado, TarefaEstadoDTO> _tarefaEstadoMap = tarefaEstadoMap;
        private readonly IToEntityMapper<Tarefa, TarefaDTO> _map = map;
        private readonly IValidador<TarefaDTO> _validador = validador;

        public async Task<Resultado<Tarefa>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO)
        {
            var erros = _validador.Validar(tarefaDTO);
            if (erros.TemErros())
                return Resultado<Tarefa>.Falhas(erros);

            var estado = await _tarefaEstadoRepository.ObterPorIdAsync(tarefaDTO.EstadoDaTarefa.Id);
            if (estado is null)
                return Resultado<Tarefa>.Falha(TarefaResource.Erro_EstadoNaoEncontrado);

            tarefaDTO.EstadoDaTarefa = _tarefaEstadoMap.MapToDto(estado);
            var tarefa = _map.MapToEntity(tarefaDTO);

            var relacionamentoResultado = await _tarefaRelacionamentoService.RelacionarAsync(tarefa, tarefaDTO);
            if (relacionamentoResultado.TeveFalha)
                return Resultado<Tarefa>.Falhas(relacionamentoResultado.Messages);

            return Resultado<Tarefa>.Sucesso(tarefa);
        }
    }
}