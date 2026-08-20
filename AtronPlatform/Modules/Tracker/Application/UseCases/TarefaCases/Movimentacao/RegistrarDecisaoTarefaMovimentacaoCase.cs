using Application.Interfaces.Mapping;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases.Movimentacao
{
    public sealed class RegistrarDecisaoTarefaMovimentacaoCase(
        ITarefaMovimentacaoRepository movimentacaoRepository,
        ITarefaMovimentacaoMapping movimentacaoMapping)
    {
        private readonly ITarefaMovimentacaoRepository _movimentacaoRepository = movimentacaoRepository;
        private readonly ITarefaMovimentacaoMapping _movimentacaoMapping = movimentacaoMapping;

        public async Task<Resultado> ExecutarAsync(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel,
            bool aprovar)
        {
            var dto = _movimentacaoMapping.MapearParaDecisao(solicitacao, responsavel, aprovar);
            var entidade = _movimentacaoMapping.MapToEntity(dto);

            if (!await _movimentacaoRepository.RegistrarAsync(entidade))
                return Resultado.Falha(TarefaResource.Erro_RegistrarMovimentacao);

            return Resultado.Sucesso();
        }
    }
}
