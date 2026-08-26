#nullable enable

namespace AtronStock.Domain.ValueObjects;

public sealed class SolicitacaoGeracaoProdutosLote
{
    private SolicitacaoGeracaoProdutosLote()
    {
    }

    public SolicitacaoGeracaoProdutosLote(
        string codigoBase,
        int quantidadeSolicitada,
        string solicitanteCodigo,
        string descricao,
        string? descricaoComplementar,
        DateTime dataAquisicao,
        decimal precoUnitario,
        IEnumerable<string> categoriaCodigos)
    {
        CodigoBase = codigoBase;
        QuantidadeSolicitada = quantidadeSolicitada;
        SolicitanteCodigo = solicitanteCodigo;
        Descricao = descricao;
        DescricaoComplementar = descricaoComplementar;
        DataAquisicao = dataAquisicao;
        PrecoUnitario = precoUnitario;
        CategoriaCodigos = (categoriaCodigos
            ?? throw new ArgumentNullException(nameof(categoriaCodigos)))
            .ToList();
    }

    public string CodigoBase { get; private set; } = string.Empty;

    public int QuantidadeSolicitada { get; private set; }

    public string SolicitanteCodigo { get; private set; } = string.Empty;

    public string Descricao { get; private set; } = string.Empty;

    public string? DescricaoComplementar { get; private set; }

    public DateTime DataAquisicao { get; private set; }

    public decimal PrecoUnitario { get; private set; }

    public List<string> CategoriaCodigos { get; private set; } = [];
}
