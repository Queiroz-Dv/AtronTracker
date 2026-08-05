using Microsoft.Extensions.Configuration;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Caching;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Shared.Tests.Application.Services.Caching;

public class CacheProviderInfoServiceTests
{
    [Fact]
    public void DeveInformarRedisComoCacheDistribuido()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cache:Provider"] = "Redis"
            })
            .Build();
        var service = new CacheProviderInfoService(
            configuration,
            new CacheServiceFake());

        var resultado = service.ObterInfo();

        Assert.Equal("Redis", resultado.ProviderConfigurado);
        Assert.True(resultado.Distribuido);
        Assert.Null(resultado.ImplementacaoMemoria);
        Assert.Null(resultado.DiretorioArquivoJson);
        Assert.Contains("compartilhado entre instancias", resultado.Observacao);
    }

    private sealed class CacheServiceFake : ICacheService
    {
        public void GravarCache<T>(CacheInfo<T> cacheInfo)
        {
        }

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
        {
        }

        public T ObterCache<T>(ChaveCache chaveCache)
            => default;

        public void RemoverCache(ChaveCache chaveCache)
        {
        }
    }
}
