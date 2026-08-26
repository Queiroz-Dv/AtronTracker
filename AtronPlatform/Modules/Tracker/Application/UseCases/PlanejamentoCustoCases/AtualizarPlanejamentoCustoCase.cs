using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.PlanejamentoCustoCases
{
    public sealed class AtualizarPlanejamentoCustoCase(
        IPlanejamentoCustoPreparacaoService preparacaoService,
        IPlanejamentoCustoRepository planejamentoCustoRepository)
    {
        private readonly IPlanejamentoCustoPreparacaoService _preparacaoService = preparacaoService;
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
