namespace AtronStock.Application.DTO.Response;

public sealed record GeracaoProdutosLoteResultado(
    int LoteProdutoId,
    string LoteProdutoCodigo,
    int QuantidadeProcessada);
