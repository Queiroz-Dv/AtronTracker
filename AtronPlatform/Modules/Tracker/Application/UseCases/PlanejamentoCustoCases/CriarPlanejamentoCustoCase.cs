using Application.DTO;
using Application.Resources;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class CriarPlanejamentoCustoCase(
        PlanejamentoCustoPreparacaoService preparacaoService,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly PlanejamentoCustoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository = planejamentoCustoRepository;

        public async Task<Resultado> ExecutarAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var preparacao = await _preparacaoService.PrepararCriacaoAsync(planejamentoCustoDTO);
            if (preparacao.TeveFalha)
                return Resultado.Falha(preparacao.Messages);

            var planejamentoPreparado = preparacao.Dados!;
            var criado = await _planejamentoCustoRepository.CriarAsync(planejamentoPreparado.Entidade);
            if (!criado)
                return Resultado.Falha(PlanejamentoCustoResource.Erro_CriarPlanejamento);

            var resultado = Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(
                    PlanejamentoCustoResource.Mensagem_PlanejamentoCriado,
                    planejamentoPreparado.Entidade.Codigo));

            AdicionarMensagensDetalhes(resultado, planejamentoPreparado.ResultadoDetalhes);
            return resultado;
        }

        private static void AdicionarMensagensDetalhes(Resultado resultado, Resultado resultadoDetalhes)
        {
            foreach (var mensagem in resultadoDetalhes.Messages)
                resultado.Adicionar(mensagem);
        }
    }
}