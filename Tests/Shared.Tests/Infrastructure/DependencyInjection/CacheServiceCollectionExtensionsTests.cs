using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Caching;
using Shared.Infrastructure.Caching;
using Shared.Infrastructure.DependencyInjection;
using Xunit;

namespace Shared.Tests.Infrastructure.DependencyInjection;

public class CacheServiceCollectionExtensionsTests
{
    [Theory]
    [InlineData(null, typeof(CacheService))]
    [InlineData("Memory", typeof(CacheService))]
    [InlineData("JsonFile", typeof(JsonFileCacheService))]
    [InlineData("ArquivoJson", typeof(JsonFileCacheService))]
    [InlineData("Redis", typeof(RedisCacheService))]
    public void DeveRegistrarSomenteAImplementacaoDoProviderSelecionado(
        string? provider,
        Type implementacaoEsperada)
    {
        var configuration = CriarConfiguracao(provider, "localhost:6379");
        var services = new ServiceCollection();

        services.AddAtronCache(configuration);

        var registro = Assert.Single(
            services,
            descriptor => descriptor.ServiceType == typeof(ICacheService));
        Assert.Equal(implementacaoEsperada, registro.ImplementationType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void DeveRejeitarRedisSemConexao(string? connectionString)
    {
        var configuration = CriarConfiguracao("Redis", connectionString);
        var services = new ServiceCollection();

        var exception = Assert.Throws<InvalidOperationException>(
            () => services.AddAtronCache(configuration));

        Assert.Equal("A conexão do Redis não foi configurada.", exception.Message);
    }

    private static IConfiguration CriarConfiguracao(
        string? provider,
        string? connectionString)
    {
        var valores = new Dictionary<string, string?>
        {
            ["Cache:Provider"] = provider,
            ["Cache:Redis:ConnectionString"] = connectionString,
            ["Cache:Redis:InstanceName"] = "atron:test:"
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(valores)
            .Build();
    }
}
