using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class ExcluirPlanejamentoCustoCase(
        IPlanejamentoCustoPreparacaoService preparacaoService,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly IPlanejamentoCustoPreparacaoService _preparacaoService = preparacaoService;
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
