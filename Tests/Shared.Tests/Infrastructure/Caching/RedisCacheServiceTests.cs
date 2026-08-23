using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Infrastructure.Caching;
using StackExchange.Redis;
using Xunit;

namespace Shared.Tests.Infrastructure.Caching;

public class RedisCacheServiceTests
{
    [Fact]
    public void DeveGravarERecuperarObjetoSerializadoComExpiracao()
    {
        var cacheDistribuido = new CacheDistribuidoFake();
        var service = CriarService(cacheDistribuido);
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
        var service = CriarService(cacheDistribuido);

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
        var service = CriarService(new CacheDistribuidoFake());

        var resultado = service.ObterCache<DadosTeste>(
            new ChaveCache(ECacheKeysInfo.Acesso, "INEXISTENTE"));

        Assert.Null(resultado);
    }

    [Fact]
    public void DeveRemoverChave()
    {
        var cacheDistribuido = new CacheDistribuidoFake();
        var service = CriarService(cacheDistribuido);
        var chave = new ChaveCache(ECacheKeysInfo.Acesso, "USR001");

        service.GravarCache(new CacheInfo<DadosTeste>(chave)
        {
            EntityInfo = new DadosTeste("USR001", "Usuario de teste")
        });
        service.RemoverCache(chave);

        Assert.Null(service.ObterCache<DadosTeste>(chave));
    }

    [Fact]
    public void DeveIgnorarIndisponibilidadeDoRedisEmDesenvolvimento()
    {
        var logger = new LoggerFake<RedisCacheService>();
        var service = CriarService(
            new CacheDistribuidoIndisponivelFake(),
            Environments.Development,
            logger);
        var chave = new ChaveCache(ECacheKeysInfo.Acesso, "USR001");

        var excecao = Record.Exception(() =>
        {
            service.GravarCache(new CacheInfo<DadosTeste>(chave)
            {
                EntityInfo = new DadosTeste("USR001", "Usuario de teste")
            });
            Assert.Null(service.ObterCache<DadosTeste>(chave));
            service.RemoverCache(chave);
        });

        Assert.Null(excecao);
        Assert.Contains(
            logger.Mensagens,
            mensagem => mensagem.Contains("Inicie o Redis local"));
    }

    [Fact]
    public void DevePropagarIndisponibilidadeDoRedisForaDeDesenvolvimento()
    {
        var service = CriarService(
            new CacheDistribuidoIndisponivelFake(),
            Environments.Production);
        var chave = new ChaveCache(ECacheKeysInfo.Acesso, "USR001");

        Assert.Throws<RedisConnectionException>(() =>
            service.GravarCache(new CacheInfo<DadosTeste>(chave)
            {
                EntityInfo = new DadosTeste("USR001", "Usuario de teste")
            }));
    }

    private static RedisCacheService CriarService(
        IDistributedCache cacheDistribuido,
        string ambiente = "Development",
        ILogger<RedisCacheService>? logger = null)
    {
        return new RedisCacheService(
            cacheDistribuido,
            new HostEnvironmentFake { EnvironmentName = ambiente },
            logger ?? NullLogger<RedisCacheService>.Instance);
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

    private sealed class CacheDistribuidoIndisponivelFake : IDistributedCache
    {
        public byte[]? Get(string key) => throw CriarExcecao();

        public Task<byte[]?> GetAsync(string key, CancellationToken token = default)
            => throw CriarExcecao();

        public void Refresh(string key) => throw CriarExcecao();

        public Task RefreshAsync(string key, CancellationToken token = default)
            => throw CriarExcecao();

        public void Remove(string key) => throw CriarExcecao();

        public Task RemoveAsync(string key, CancellationToken token = default)
            => throw CriarExcecao();

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
            => throw CriarExcecao();

        public Task SetAsync(
            string key,
            byte[] value,
            DistributedCacheEntryOptions options,
            CancellationToken token = default)
            => throw CriarExcecao();

        private static RedisConnectionException CriarExcecao()
            => new(ConnectionFailureType.UnableToConnect, "Redis indisponível para o teste.");
    }

    private sealed class HostEnvironmentFake : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = Environments.Development;
        public string ApplicationName { get; set; } = "Shared.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = null!;
    }

    private sealed class LoggerFake<T> : ILogger<T>
    {
        public List<string> Mensagens { get; } = [];

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull
            => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (logLevel == LogLevel.Warning)
                Mensagens.Add(formatter(state, exception));
        }
    }
}
