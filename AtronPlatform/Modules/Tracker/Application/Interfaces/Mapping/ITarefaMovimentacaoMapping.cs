using Application.DTO;
using Application.Records.Tarefa;
using Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace Application.Interfaces.Mapping
{
    public interface ITarefaMovimentacaoMapping
        : IMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>
    {
        TarefaMovimentacaoDTO MapearParaCriacao(
            Tarefa tarefa,
            Usuario responsavel);

        TarefaMovimentacaoDTO MapearParaAtualizacao(
            AtualizacaoMovimentacaoRecord parametros);

        TarefaMovimentacaoDTO MapearParaObtencao(
            Tarefa tarefa,
            Usuario responsavel);

        TarefaMovimentacaoDTO MapearParaSolicitacao(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel);

        TarefaMovimentacaoDTO MapearParaDecisao(
            SolicitacaoObtencaoTarefa solicitacao,
            Usuario responsavel,
            bool aprovar);
    }
}
