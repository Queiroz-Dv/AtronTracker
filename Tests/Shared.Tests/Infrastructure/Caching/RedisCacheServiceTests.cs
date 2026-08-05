using Microsoft.Extensions.Caching.Distributed;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Caching;
using Xunit;

namespace Shared.Tests.Infrastructure.Caching;

public class RedisCacheServiceTests
{
    [Fact]
    public void DeveGravarERecuperarObjetoSerializadoComExpiracao()
    {
        var cacheDistribuido = new CacheDistribuidoFake();
        var service = new RedisCacheService(cacheDistribuido);
        var chave = new ChaveCache(ECacheKeysInfo.Acesso, "USR001");
        var expiracao = TimeSpan.FromMinutes(5);

        service.GravarCache(new CacheInfo<DadosTeste>(chave)
        {
            EntityInfo = new DadosTeste("USR001", "Usuario de teste")
        }, expiracao);

        var resultado = service.ObterCache<DadosTeste>(chave);

        Assert.Equal(new DadosTeste("USR001", "Usuario de teste"), resultado);
        Assert.Equal(expiracao, cacheDistribuido.UltimasOpcoes?.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public void DeveUsarTrintaMinutosQuandoExpiracaoNaoFoiInformada()
    {
        var cacheDistribuido = new CacheDistribuidoFake();
        var service = new RedisCacheService(cacheDistribuido);

        service.GravarCache(new CacheInfo<DadosTeste>(
            new ChaveCache(ECacheKeysInfo.Acesso, "USR001"))
        {
            EntityInfo = new DadosTeste("USR001", "Usuario de teste")
        });

        Assert.Equal(TimeSpan.FromMinutes(30), cacheDistribuido.UltimasOpcoes?.AbsoluteExpirationRelativeToNow);
    }

    [Fact]
    public void DeveRetornarNuloQuandoChaveNaoExiste()
    {
        var service = new RedisCacheService(new CacheDistribuidoFake());

        var resultado = service.ObterCache<DadosTeste>(
            new ChaveCache(ECacheKeysInfo.Acesso, "INEXISTENTE"));

        Assert.Null(resultado);
    }

    [Fact]
    public void DeveRemoverChave()
    {
        var cacheDistribuido = new CacheDistribuidoFake();
        var service = new RedisCacheService(cacheDistribuido);
        var chave = new ChaveCache(ECacheKeysInfo.Acesso, "USR001");

        service.GravarCache(new CacheInfo<DadosTeste>(chave)
        {
            EntityInfo = new DadosTeste("USR001", "Usuario de teste")
        });
        service.RemoverCache(chave);

        Assert.Null(service.ObterCache<DadosTeste>(chave));
    }

    private sealed record DadosTeste(string Codigo, string Nome);

    private sealed class CacheDistribuidoFake : IDistributedCache
    {
        private readonly Dictionary<string, byte[]> _itens = [];

        public DistributedCacheEntryOptions? UltimasOpcoes { get; private set; }

        public byte[]? Get(string key)
            => _itens.TryGetValue(key, out var valor) ? valor : null;

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            => Task.FromResult(Get(key));

        public void Refresh(string key)
        {
        }

        public Task RefreshAsync(string key, CancellationToken token = default)
            => Task.CompletedTask;

        public void Remove(string key)
            => _itens.Remove(key);

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            _itens[key] = value;
            UltimasOpcoes = options;
        }

        public Task SetAsync(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
        {
            Set(key, value, options);
            return Task.CompletedTask;
        }
    }
}
