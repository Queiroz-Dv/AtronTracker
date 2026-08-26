#nullable enable

namespace AtronStock.Domain.ValueObjects;

public sealed class ResultadoProcessamentoProdutoLote
{
    public int QuantidadeProcessada { get; private set; }

    public string? Erro { get; private set; }

    internal void Concluir(int quantidadeProcessada)
    {
        QuantidadeProcessada = quantidadeProcessada;
        Erro = null;
    }

    internal void Falhar(string erro)
    {
        Erro = erro.Length <= 2000 ? erro : erro[..2000];
    }

    internal void LimparErro()
        => Erro = null;
}
