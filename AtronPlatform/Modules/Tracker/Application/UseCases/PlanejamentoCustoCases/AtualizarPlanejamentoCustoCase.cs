using Application.DTO;
using Application.Resources;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class AtualizarPlanejamentoCustoCase(
        PlanejamentoCustoPreparacaoService preparacaoService,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly PlanejamentoCustoPreparacaoService _preparacaoService = preparacaoService;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository = planejamentoCustoRepository;

        public async Task<Resultado> ExecutarAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var preparacao = await _preparacaoService.PrepararAtualizacaoAsync(codigo, planejamentoCustoDTO);
            if (preparacao.TeveFalha)
                return Resultado.Falha(preparacao.Messages);

            var planejamentoPreparado = preparacao.Dados!;
            var atualizado = await _planejamentoCustoRepository.AtualizarAsync(planejamentoPreparado.Entidade);
            if (!atualizado)
                return Resultado.Falha(string.Format(PlanejamentoCustoResource.Erro_AtualizarPlanejamento, codigo));

            var resultado = Resultado
                .Sucesso()
                .AdicionarMensagem(string.Format(PlanejamentoCustoResource.Mensagem_PlanejamentoAtualizado, codigo));

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