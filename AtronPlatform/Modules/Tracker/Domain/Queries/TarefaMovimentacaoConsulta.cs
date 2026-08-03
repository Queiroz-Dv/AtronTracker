using Domain.Entities;
using System.Collections.Generic;

namespace Domain.Queries
{
    public sealed record TarefaMovimentacaoConsulta(
        int TarefaId,
        int Pagina,
        int TamanhoPagina);

    public sealed record TarefaMovimentacaoPagina(
        IReadOnlyCollection<TarefaMovimentacao> Itens,
        int TotalItens);
}
