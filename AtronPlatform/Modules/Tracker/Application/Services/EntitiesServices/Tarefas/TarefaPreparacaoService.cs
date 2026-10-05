using Application.DTO;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaPreparacaoService
    {
        private readonly TarefaRelacionamentoService _tarefaRelacionamentoService;
        private readonly ITarefaEstadoRepository _tarefaEstadoRepository;
        private readonly IMapper<TarefaEstado, TarefaEstadoDTO> _tarefaEstadoMap;
        private readonly IToEntityMapper<Tarefa, TarefaDTO> _map;
        private readonly IValidador<TarefaDTO> _validador;

        protected TarefaPreparacaoService() { }

        public TarefaPreparacaoService(
            TarefaRelacionamentoService tarefaRelacionamentoService,
            ITarefaEstadoRepository tarefaEstadoRepository,
            IMapper<TarefaEstado, TarefaEstadoDTO> tarefaEstadoMap,
            IToEntityMapper<Tarefa, TarefaDTO> map,
            IValidador<TarefaDTO> validador)
        {
            _tarefaRelacionamentoService = tarefaRelacionamentoService;
            _tarefaEstadoRepository = tarefaEstadoRepository;
            _tarefaEstadoMap = tarefaEstadoMap;
            _map = map;
            _validador = validador;
        }

        public virtual async Task<Resultado<Tarefa>> PrepararParaPersistenciaAsync(TarefaDTO tarefaDTO)
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