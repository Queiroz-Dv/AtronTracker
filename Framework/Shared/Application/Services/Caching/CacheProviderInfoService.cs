using Microsoft.Extensions.Configuration;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using System;
using System.IO;

namespace Shared.Application.Services.Caching
{
    public class CacheProviderInfoService : ICacheProviderInfoService
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public CacheProviderInfoService(
            IConfiguration configuration,
            ICacheService cacheService)
        {
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public CacheProviderInfoDTO ObterInfo()
        {
            var provider = _configuration["Cache:Provider"];
            if (string.IsNullOrWhiteSpace(provider))
                provider = "Memory";

            var providerNormalizado = NormalizarProvider(provider);
            var diretorioArquivoJson = ObterDiretorioArquivoJson();

            return new CacheProviderInfoDTO
            {
                ProviderConfigurado = providerNormalizado,
                ImplementacaoCache = _cacheService.GetType().Name,
                ImplementacaoMemoria = providerNormalizado == "Memory" ? "MemoryCache" : null,
                DiretorioArquivoJson = providerNormalizado == "JsonFile" ? diretorioArquivoJson : null,
                Distribuido = providerNormalizado == "Redis",
                Observacao = ObterObservacao(providerNormalizado, diretorioArquivoJson)
            };
        }

        private static string NormalizarProvider(string provider)
        {
            if (string.Equals(provider, "JsonFile", StringComparison.OrdinalIgnoreCase)
                || string.Equals(provider, "ArquivoJson", StringComparison.OrdinalIgnoreCase))
                return "JsonFile";

            if (string.Equals(provider, "Redis", StringComparison.OrdinalIgnoreCase))
                return "Redis";

            return "Memory";
        }

        private string ObterDiretorioArquivoJson()
        {
            var diretorio = _configuration["Cache:JsonFile:Diretorio"];
            diretorio = string.IsNullOrWhiteSpace(diretorio)
                ? Path.Combine(AppContext.BaseDirectory, "cache-json")
                : diretorio;

            return Path.IsPathRooted(diretorio)
                ? diretorio
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, diretorio));
        }

        private static string ObterObservacao(string provider, string diretorioArquivoJson)
        {
            if (provider == "JsonFile")
                return $"Cache local em arquivos JSON no diretorio '{diretorioArquivoJson}'. Sobrevive a reinicio da aplicacao, mas nao e compartilhado entre instancias.";

            if (provider == "Redis")
                return "Cache distribuido Redis compativel. E compartilhado entre instancias e permanece independente do processo da aplicacao.";

            return "Cache local em memoria do processo. Sera perdido em reinicio e nao e compartilhado entre instancias.";
        }
    }
}
