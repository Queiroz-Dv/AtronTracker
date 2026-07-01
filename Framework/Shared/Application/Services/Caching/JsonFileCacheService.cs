using Microsoft.Extensions.Configuration;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Shared.Application.Services.Caching
{
    public class JsonFileCacheService : ICacheService
    {
        private static readonly object Sincronizador = new();
        private readonly string _diretorioCache;

        public JsonFileCacheService(IConfiguration configuration)
        {
            var diretorioConfigurado = configuration["Cache:JsonFile:Diretorio"];
            var diretorio = string.IsNullOrWhiteSpace(diretorioConfigurado)
                ? Path.Combine(AppContext.BaseDirectory, "cache-json")
                : diretorioConfigurado;

            _diretorioCache = Path.IsPathRooted(diretorio)
                ? diretorio
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, diretorio));
        }

        public void GravarCache<T>(CacheInfo<T> cacheInfo)
        {
            GravarCache(cacheInfo, ObterExpiracaoRelativa(cacheInfo));
        }

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
        {
            var payload = new JsonFileCachePayload<T>
            {
                Chave = cacheInfo.KeyDescription,
                ExpiraEmUtc = DateTime.UtcNow.Add(expiracao),
                Dados = cacheInfo.EntityInfo
            };

            var arquivo = ObterCaminhoArquivo(cacheInfo.KeyDescription);
            var json = JsonSerializer.Serialize(payload);

            lock (Sincronizador)
            {
                Directory.CreateDirectory(_diretorioCache);
                File.WriteAllText(arquivo, json, Encoding.UTF8);
            }
        }

        public T ObterCache<T>(string cacheKey)
        {
            var arquivo = ObterCaminhoArquivo(cacheKey);
            if (!File.Exists(arquivo))
                return default;

            lock (Sincronizador)
            {
                try
                {
                    var json = File.ReadAllText(arquivo, Encoding.UTF8);
                    var payload = JsonSerializer.Deserialize<JsonFileCachePayload<JsonElement>>(json);

                    if (payload is null || payload.ExpiraEmUtc <= DateTime.UtcNow)
                    {
                        RemoverArquivo(arquivo);
                        return default;
                    }

                    if (!string.Equals(payload.Chave, cacheKey, StringComparison.Ordinal))
                        return default;

                    return payload.Dados.Deserialize<T>();
                }
                catch
                {
                    RemoverArquivo(arquivo);
                    return default;
                }
            }
        }

        public void RemoverCache(ECacheKeysInfo chave)
        {
            RemoverPorChave(chave.GetDescription());
        }

        public void RemoverCache(ECacheKeysInfo chave, string codigoDaEntidade)
        {
            RemoverPorChave($"{chave.GetDescription()}:{codigoDaEntidade}");
        }

        private void RemoverPorChave(string cacheKey)
        {
            var arquivo = ObterCaminhoArquivo(cacheKey);

            lock (Sincronizador)
            {
                RemoverArquivo(arquivo);
            }
        }

        private string ObterCaminhoArquivo(string cacheKey)
        {
            var nomeArquivo = $"{CriarHash(cacheKey)}.json";
            return Path.Combine(_diretorioCache, nomeArquivo);
        }

        private static string CriarHash(string valor)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor ?? string.Empty));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        private static void RemoverArquivo(string arquivo)
        {
            if (File.Exists(arquivo))
                File.Delete(arquivo);
        }

        private static TimeSpan ObterExpiracaoRelativa<T>(CacheInfo<T> cacheInfo)
        {
            if (cacheInfo.ExpireTime == default)
                return TimeSpan.FromMinutes(30);

            var expireTimeUtc = cacheInfo.ExpireTime.Kind == DateTimeKind.Local
                ? cacheInfo.ExpireTime.ToUniversalTime()
                : cacheInfo.ExpireTime;

            var ttl = expireTimeUtc - DateTime.UtcNow;
            return ttl > TimeSpan.Zero ? ttl : TimeSpan.FromSeconds(1);
        }
    }

    internal class JsonFileCachePayload<T>
    {
        public string Chave { get; set; }
        public DateTime ExpiraEmUtc { get; set; }
        public T Dados { get; set; }
    }
}
