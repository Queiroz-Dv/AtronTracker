using AtronStock.Domain.Enums;

namespace AtronStock.Application.DTO.Response;

public sealed record SolicitacaoGeracaoProdutosLoteResponse(
    int ProcessamentoId,
    EStatusProcessamentoProdutoLote Status);
