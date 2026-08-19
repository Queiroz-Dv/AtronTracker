using Application.Interfaces.Mapping;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases.Movimentacao
{
    public sealed class RegistrarObtencaoTarefaMovimentacaoCase(
        ITarefaMovimentacaoRepository movimentacaoRepository,
        ITarefaMovimentacaoMapping movimentacaoMapping)
    {
        private readonly ITarefaMovimentacaoRepository _movimentacaoRepository = movimentacaoRepository;
        private readonly ITarefaMovimentacaoMapping _movimentacaoMapping = movimentacaoMapping;

        public async Task<Resultado> ExecutarAsync(Tarefa tarefa, Usuario responsavel)
        {
            var dto = _movimentacaoMapping.MapearParaObtencao(tarefa, responsavel);
            var entidade = _movimentacaoMapping.MapToEntity(dto);

            return await _movimentacaoRepository.RegistrarAsync(entidade)
                ? Resultado.Sucesso()
                : Resultado.Falha(TarefaResource.Erro_RegistrarMovimentacao);
        }
    }
}