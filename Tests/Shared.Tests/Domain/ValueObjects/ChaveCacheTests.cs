using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Shared.Tests.Domain.ValueObjects;

public class ChaveCacheTests
{
    [Theory]
    [InlineData(ECacheKeysInfo.Acesso, "acesso")]
    [InlineData(ECacheKeysInfo.DadosTemporarios, "dadosTemporarios")]
    public void DeveCriarChaveSemIdentificador(ECacheKeysInfo chave, string esperado)
    {
        var chaveCache = new ChaveCache(chave);

        Assert.Equal(esperado, chaveCache.Descricao);
    }

    [Fact]
    public void DeveCriarChaveComIdentificador()
    {
        var chaveCache = new ChaveCache(ECacheKeysInfo.DadosTemporarios, "abc123");

        Assert.Equal("dadosTemporarios:abc123", chaveCache.Descricao);
    }
}
