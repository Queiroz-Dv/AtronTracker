using Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EnderecoTests
{
    [Fact]
    public void EnderecosComMesmoLogradouro_DevemSerIguaisPorValor()
    {
        var primeiro = new Endereco { Logradouro = "Rua de Teste" };
        var segundo = new Endereco { Logradouro = "Rua de Teste" };

        Assert.NotSame(primeiro, segundo);
        Assert.Equal(primeiro, segundo);
        Assert.Equal(primeiro.GetHashCode(), segundo.GetHashCode());
    }

    [Fact]
    public void EnderecosComLogradourosDiferentes_NaoDevemSerIguais()
    {
        Assert.NotEqual(
            new Endereco { Logradouro = "Rua A" },
            new Endereco { Logradouro = "Rua B" });
    }

    [Fact]
    public void SubstituirLogradouro_NaoDeveAlterarValorOriginal()
    {
        var original = new Endereco { Logradouro = "Rua A" };

        var atualizado = original with { Logradouro = "Rua B" };

        Assert.Equal("Rua A", original.Logradouro);
        Assert.Equal("Rua B", atualizado.Logradouro);
    }
}
