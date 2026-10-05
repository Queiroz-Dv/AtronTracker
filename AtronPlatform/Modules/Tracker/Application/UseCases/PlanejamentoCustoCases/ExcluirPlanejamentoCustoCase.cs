using Application.Resources;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class ExcluirPlanejamentoCustoCase(
        PlanejamentoCustoPreparacaoService preparacaoService,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly PlanejamentoCustoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository = planejamentoCustoRepository;

        public async Task<Resultado> ExecutarAsync(string codigo)
        {
            var preparacao = await _preparacaoService.PrepararRemocaoAsync(codigo);
            if (preparacao.TeveFalha)
                return Resultado.Falha(preparacao.Messages);

            var removido = await _planejamentoCustoRepository.RemoverAsync(preparacao.Dados!);
            if (!removido)
                return Resultado.Falha(string.Format(PlanejamentoCustoResource.Erro_RemoverPlanejamento, codigo));

            return Resultado
                .Sucesso()
                .AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
        }
    }
}