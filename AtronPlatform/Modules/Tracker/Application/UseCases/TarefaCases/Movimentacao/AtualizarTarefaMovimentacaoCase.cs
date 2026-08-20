using Application.Interfaces.Mapping;
using Application.Records.Tarefa;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases.Movimentacao
{
    public sealed class AtualizarTarefaMovimentacaoCase(
        ITarefaMovimentacaoRepository movimentacaoRepository,
        ITarefaMovimentacaoMapping movimentacaoMapping)
    {
        private readonly ITarefaMovimentacaoRepository _movimentacaoRepository = movimentacaoRepository;
        private readonly ITarefaMovimentacaoMapping _movimentacaoMapping = movimentacaoMapping;

        public async Task<Resultado> ExecutarAsync(
            AtualizacaoMovimentacaoRecord parametros)
        {
            var dto = _movimentacaoMapping.MapearParaAtualizacao(parametros);
            var entidade = _movimentacaoMapping.MapToEntity(dto);

            return await _movimentacaoRepository.RegistrarAsync(entidade)
                ? Resultado.Sucesso()
                : Resultado.Falha(TarefaResource.Erro_RegistrarMovimentacao);
        }
    }
}
