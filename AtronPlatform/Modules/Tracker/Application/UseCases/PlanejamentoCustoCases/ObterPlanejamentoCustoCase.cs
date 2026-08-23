using Application.DTO;
using Application.Mapping;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class ObterPlanejamentoCustoCase(
        PlanejamentoCustoMapping planejamentoCustoMapping,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly PlanejamentoCustoMapping _planejamentoCustoMapping = planejamentoCustoMapping;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository = planejamentoCustoRepository;

        public async Task<Resultado<PlanejamentoCustoDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(codigo);
            return planejamento == null
                ? Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)
                : Resultado<PlanejamentoCustoDTO>.Sucesso(_planejamentoCustoMapping.MapToDto(planejamento));
        }

        public async Task<Resultado<List<PlanejamentoCustoDTO>>> ObterPorAnoAsync(int ano)
        {
            var planejamentos = await _planejamentoCustoRepository.ObterPorAnoAsync(ano);
            var dtos = _planejamentoCustoMapping.MapToDtos(planejamentos).ToList();
            return Resultado<List<PlanejamentoCustoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<PlanejamentoCustoDTO>>> ObterTodosAsync()
        {
            var planejamentos = await _planejamentoCustoRepository.ObterTodosAsync();
            var dtos = _planejamentoCustoMapping.MapToDtos(planejamentos).ToList();
            return Resultado<List<PlanejamentoCustoDTO>>.Sucesso(dtos);
        }
    }
}
