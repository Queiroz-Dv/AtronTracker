using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller para diagnosticar cache e cookies sem depender do fluxo de login.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DiagnosticoController : ControllerBase
    {
        private const string NomeCookieDiagnostico = "ATRON_DIAGNOSTICO_COOKIE";
        private const string HeaderChaveDiagnostico = "X-DIAGNOSTICO-CHAVE";

        private readonly ICacheService _cacheService;
        private readonly IDataProtector _protector;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ICacheProviderInfoService _cacheProviderInfoService;

        public DiagnosticoController(
            ICacheService cacheService,
            IDataProtectionProvider dataProtectionProvider,
            IConfiguration configuration,
            IWebHostEnvironment environment,
            ICacheProviderInfoService cacheProviderInfoService)
        {
            _cacheService = cacheService;
            _protector = dataProtectionProvider.CreateProtector("DiagnosticoCookieProtector");
            _configuration = configuration;
            _environment = environment;
            _cacheProviderInfoService = cacheProviderInfoService;
        }

        [HttpGet("Status")]
        public ActionResult<DiagnosticoStatusResponse> Status()
        {
            if (!DiagnosticoHabilitado())
                return NotFound();

            if (!ChaveValida())
                return Unauthorized("Chave de diagnostico invalida.");

            var cacheInfo = _cacheProviderInfoService.ObterInfo();

            return Ok(new DiagnosticoStatusResponse
            {
                Ativo = true,
                Ambiente = _environment.EnvironmentName,
                Cache = cacheInfo.ImplementacaoCache,
                CacheProviderConfigurado = cacheInfo.ProviderConfigurado,
                CacheImplementacaoMemoria = cacheInfo.ImplementacaoMemoria,
                CacheDiretorioArquivoJson = cacheInfo.DiretorioArquivoJson,
                CacheDistribuido = cacheInfo.Distribuido,
                CacheObservacao = cacheInfo.Observacao,
                CookieDiagnostico = NomeCookieDiagnostico,
                RequerChave = !string.IsNullOrWhiteSpace(ObterChaveConfigurada()),
                HorarioServidorUtc = DateTime.UtcNow,
                HorarioServidorLocal = DateTime.Now
            });
        }

        [HttpPost("Cache/Gravar")]
        public ActionResult<DiagnosticoCacheResponse> GravarCache([FromBody] DiagnosticoCacheGravarRequest request)
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            if (request is null || string.IsNullOrWhiteSpace(request.Chave))
                return BadRequest("Informe a chave do cache.");

            var ttlSegundos = request.TtlSegundos <= 0 ? 120 : request.TtlSegundos;
            var cacheInfo = CriarCacheInfo(request.Chave, request.Valor ?? string.Empty);

            _cacheService.GravarCache(cacheInfo, TimeSpan.FromSeconds(ttlSegundos));

            return Ok(new DiagnosticoCacheResponse
            {
                Chave = cacheInfo.KeyDescription,
                Status = "gravado",
                ValorEncontrado = true,
                ValorPreview = CriarPreview(request.Valor),
                TtlSegundos = ttlSegundos,
                HorarioServidorUtc = DateTime.UtcNow
            });
        }

        [HttpGet("Cache/Ler/{chave}")]
        public ActionResult<DiagnosticoCacheResponse> LerCache(string chave)
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            if (string.IsNullOrWhiteSpace(chave))
                return BadRequest("Informe a chave do cache.");

            var cacheInfo = CriarCacheInfo(chave, string.Empty);
            var valor = _cacheService.ObterCache<string>(cacheInfo.KeyDescription);

            return Ok(new DiagnosticoCacheResponse
            {
                Chave = cacheInfo.KeyDescription,
                Status = valor is null ? "nao_encontrado" : "encontrado",
                ValorEncontrado = valor is not null,
                ValorPreview = CriarPreview(valor),
                HorarioServidorUtc = DateTime.UtcNow
            });
        }

        [HttpDelete("Cache/Remover/{chave}")]
        public ActionResult<DiagnosticoCacheResponse> RemoverCache(string chave)
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            if (string.IsNullOrWhiteSpace(chave))
                return BadRequest("Informe a chave do cache.");

            var cacheInfo = CriarCacheInfo(chave, string.Empty);
            _cacheService.RemoverCache(ECacheKeysInfo.Sessao, chave);

            return Ok(new DiagnosticoCacheResponse
            {
                Chave = cacheInfo.KeyDescription,
                Status = "removido",
                ValorEncontrado = false,
                HorarioServidorUtc = DateTime.UtcNow
            });
        }

        [HttpPost("Cache/Testar")]
        public ActionResult<DiagnosticoCacheTesteResponse> TestarCache([FromBody] DiagnosticoCacheTestarRequest request)
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            var chave = string.IsNullOrWhiteSpace(request?.Chave)
                ? $"teste-{Guid.NewGuid():N}"
                : request.Chave;

            var valor = string.IsNullOrWhiteSpace(request?.Valor)
                ? $"valor-{Guid.NewGuid():N}"
                : request.Valor;

            var ttlSegundos = request?.TtlSegundos > 0 ? request.TtlSegundos : 120;
            var cacheInfo = CriarCacheInfo(chave, valor);

            _cacheService.GravarCache(cacheInfo, TimeSpan.FromSeconds(ttlSegundos));
            var valorLido = _cacheService.ObterCache<string>(cacheInfo.KeyDescription);
            var leituraOk = string.Equals(valorLido, valor, StringComparison.Ordinal);

            _cacheService.RemoverCache(ECacheKeysInfo.Sessao, chave);
            var valorDepoisRemocao = _cacheService.ObterCache<string>(cacheInfo.KeyDescription);
            var remocaoOk = valorDepoisRemocao is null;

            var cacheInfoProvider = _cacheProviderInfoService.ObterInfo();

            return Ok(new DiagnosticoCacheTesteResponse
            {
                Sucesso = leituraOk && remocaoOk,
                Chave = cacheInfo.KeyDescription,
                ProviderConfigurado = cacheInfoProvider.ProviderConfigurado,
                ImplementacaoCache = cacheInfoProvider.ImplementacaoCache,
                DiretorioArquivoJson = cacheInfoProvider.DiretorioArquivoJson,
                GravacaoOk = true,
                LeituraOk = leituraOk,
                RemocaoOk = remocaoOk,
                ValorPreview = CriarPreview(valorLido),
                TtlSegundos = ttlSegundos,
                HorarioServidorUtc = DateTime.UtcNow
            });
        }

        [HttpPost("Cookie/Gravar")]
        public ActionResult<DiagnosticoCookieResponse> GravarCookie([FromBody] DiagnosticoCookieGravarRequest request)
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            var minutos = request?.MinutosExpiracao > 0 ? request.MinutosExpiracao : 10;
            var payload = new DiagnosticoCookiePayload
            {
                Valor = request?.Valor ?? "cookie-diagnostico",
                CriadoEmUtc = DateTime.UtcNow,
                ExpiraEmUtc = DateTime.UtcNow.AddMinutes(minutos)
            };

            var json = JsonSerializer.Serialize(payload);
            var protegido = _protector.Protect(json);

            Response.Cookies.Append(NomeCookieDiagnostico, protegido, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = payload.ExpiraEmUtc
            });

            return Ok(new DiagnosticoCookieResponse
            {
                CookieEncontrado = true,
                CookieCriado = true,
                NomeCookie = NomeCookieDiagnostico,
                ConsegueDesproteger = true,
                Fingerprint = CriarFingerprint(json),
                ExpiraEmUtc = payload.ExpiraEmUtc,
                HorarioServidorUtc = DateTime.UtcNow,
                Aviso = Request.IsHttps ? null : "Cookie Secure foi criado, mas o navegador pode ignora-lo em HTTP."
            });
        }

        [HttpGet("Cookie/Ler")]
        public ActionResult<DiagnosticoCookieResponse> LerCookie()
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            if (!Request.Cookies.TryGetValue(NomeCookieDiagnostico, out var valorProtegido))
            {
                return Ok(new DiagnosticoCookieResponse
                {
                    CookieEncontrado = false,
                    NomeCookie = NomeCookieDiagnostico,
                    ConsegueDesproteger = false,
                    HorarioServidorUtc = DateTime.UtcNow
                });
            }

            try
            {
                var json = _protector.Unprotect(valorProtegido);
                var payload = JsonSerializer.Deserialize<DiagnosticoCookiePayload>(json);

                return Ok(new DiagnosticoCookieResponse
                {
                    CookieEncontrado = true,
                    NomeCookie = NomeCookieDiagnostico,
                    ConsegueDesproteger = true,
                    Fingerprint = CriarFingerprint(json),
                    ExpiraEmUtc = payload?.ExpiraEmUtc,
                    HorarioServidorUtc = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return Ok(new DiagnosticoCookieResponse
                {
                    CookieEncontrado = true,
                    NomeCookie = NomeCookieDiagnostico,
                    ConsegueDesproteger = false,
                    HorarioServidorUtc = DateTime.UtcNow,
                    Erro = ex.Message
                });
            }
        }

        [HttpDelete("Cookie/Remover")]
        public ActionResult<DiagnosticoCookieResponse> RemoverCookie()
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            Response.Cookies.Delete(NomeCookieDiagnostico, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new DiagnosticoCookieResponse
            {
                CookieEncontrado = false,
                NomeCookie = NomeCookieDiagnostico,
                CookieRemovido = true,
                HorarioServidorUtc = DateTime.UtcNow
            });
        }

        [HttpGet("Sessao/Sinais")]
        public ActionResult<DiagnosticoSessaoSinaisResponse> SinaisDaSessao()
        {
            if (!PodeExecutarDiagnostico())
                return NotFound();

            var usuarioCodigo = Request.Headers.TryGetValue("XUSRCD", out var headerUsuario)
                ? headerUsuario.ToString()
                : null;

            var chaveCache = string.IsNullOrWhiteSpace(usuarioCodigo)
                ? null
                : new CacheInfo<string>(ECacheKeysInfo.Acesso, usuarioCodigo).KeyDescription;

            var cacheEncontrado = false;
            if (!string.IsNullOrWhiteSpace(chaveCache))
                cacheEncontrado = _cacheService.ObterCache<object>(chaveCache) is not null;

            var cookieDiagnosticoEncontrado = Request.Cookies.ContainsKey(NomeCookieDiagnostico);
            var nomeCookieRefreshToken = string.IsNullOrWhiteSpace(usuarioCodigo)
                ? null
                : $"{usuarioCodigo}{ETokenInfo.RefreshToken.GetDescription()}".ToUpper();

            var cookieRefreshTokenEncontrado = nomeCookieRefreshToken is not null
                && Request.Cookies.ContainsKey(nomeCookieRefreshToken);

            var cacheInfo = _cacheProviderInfoService.ObterInfo();

            return Ok(new DiagnosticoSessaoSinaisResponse
            {
                Ambiente = _environment.EnvironmentName,
                PossuiHeaderUsuario = !string.IsNullOrWhiteSpace(usuarioCodigo),
                UsuarioCodigo = Mascarar(usuarioCodigo),
                PossuiCookieDiagnostico = cookieDiagnosticoEncontrado,
                NomeCookieDiagnostico = NomeCookieDiagnostico,
                PossuiCookieRefreshToken = cookieRefreshTokenEncontrado,
                NomeCookieRefreshToken = nomeCookieRefreshToken,
                ChaveCacheAcesso = chaveCache,
                CacheAcessoEncontrado = cacheEncontrado,
                Cache = cacheInfo.ImplementacaoCache,
                CacheProviderConfigurado = cacheInfo.ProviderConfigurado,
                CacheDiretorioArquivoJson = cacheInfo.DiretorioArquivoJson,
                CacheDistribuido = cacheInfo.Distribuido,
                HorarioServidorUtc = DateTime.UtcNow,
                HorarioServidorLocal = DateTime.Now
            });
        }

        private bool PodeExecutarDiagnostico()
        {
            return DiagnosticoHabilitado() && ChaveValida();
        }

        private bool DiagnosticoHabilitado()
        {
            return _environment.IsDevelopment() || _configuration.GetValue<bool>("Diagnostico:Habilitado");
        }

        private bool ChaveValida()
        {
            var chaveConfigurada = ObterChaveConfigurada();
            if (string.IsNullOrWhiteSpace(chaveConfigurada))
                return true;

            return Request.Headers.TryGetValue(HeaderChaveDiagnostico, out var chaveRecebida)
                && string.Equals(chaveRecebida.ToString(), chaveConfigurada, StringComparison.Ordinal);
        }

        private string ObterChaveConfigurada()
        {
            return _configuration["Diagnostico:Chave"];
        }

        private static CacheInfo<string> CriarCacheInfo(string chave, string valor)
        {
            return new CacheInfo<string>(ECacheKeysInfo.Sessao, chave)
            {
                EntityInfo = valor
            };
        }

        private static string CriarPreview(string valor)
        {
            if (string.IsNullOrEmpty(valor))
                return valor;

            return valor.Length <= 12 ? valor : $"{valor[..6]}...{valor[^3..]}";
        }

        private static string CriarFingerprint(string valor)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(valor ?? string.Empty));
            return Convert.ToHexString(hash)[..16].ToLowerInvariant();
        }

        private static string Mascarar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return valor.Length <= 2 ? "**" : $"{valor[0]}***{valor[^1]}";
        }
    }

    public class DiagnosticoCacheGravarRequest
    {
        public string Chave { get; set; }
        public string Valor { get; set; }
        public int TtlSegundos { get; set; }
    }

    public class DiagnosticoCacheTestarRequest
    {
        public string Chave { get; set; }
        public string Valor { get; set; }
        public int TtlSegundos { get; set; }
    }

    public class DiagnosticoCookieGravarRequest
    {
        public string Valor { get; set; }
        public int MinutosExpiracao { get; set; }
    }

    public class DiagnosticoStatusResponse
    {
        public bool Ativo { get; set; }
        public string Ambiente { get; set; }
        public string Cache { get; set; }
        public string CacheProviderConfigurado { get; set; }
        public string CacheImplementacaoMemoria { get; set; }
        public string CacheDiretorioArquivoJson { get; set; }
        public bool CacheDistribuido { get; set; }
        public string CacheObservacao { get; set; }
        public string CookieDiagnostico { get; set; }
        public bool RequerChave { get; set; }
        public DateTime HorarioServidorUtc { get; set; }
        public DateTime HorarioServidorLocal { get; set; }
    }

    public class DiagnosticoCacheResponse
    {
        public string Chave { get; set; }
        public string Status { get; set; }
        public bool ValorEncontrado { get; set; }
        public string ValorPreview { get; set; }
        public int TtlSegundos { get; set; }
        public DateTime HorarioServidorUtc { get; set; }
    }

    public class DiagnosticoCacheTesteResponse
    {
        public bool Sucesso { get; set; }
        public string Chave { get; set; }
        public string ProviderConfigurado { get; set; }
        public string ImplementacaoCache { get; set; }
        public string DiretorioArquivoJson { get; set; }
        public bool GravacaoOk { get; set; }
        public bool LeituraOk { get; set; }
        public bool RemocaoOk { get; set; }
        public string ValorPreview { get; set; }
        public int TtlSegundos { get; set; }
        public DateTime HorarioServidorUtc { get; set; }
    }

    public class DiagnosticoCookieResponse
    {
        public bool CookieEncontrado { get; set; }
        public bool CookieCriado { get; set; }
        public bool CookieRemovido { get; set; }
        public string NomeCookie { get; set; }
        public bool ConsegueDesproteger { get; set; }
        public string Fingerprint { get; set; }
        public DateTime? ExpiraEmUtc { get; set; }
        public DateTime HorarioServidorUtc { get; set; }
        public string Aviso { get; set; }
        public string Erro { get; set; }
    }

    public class DiagnosticoSessaoSinaisResponse
    {
        public string Ambiente { get; set; }
        public bool PossuiHeaderUsuario { get; set; }
        public string UsuarioCodigo { get; set; }
        public bool PossuiCookieDiagnostico { get; set; }
        public string NomeCookieDiagnostico { get; set; }
        public bool PossuiCookieRefreshToken { get; set; }
        public string NomeCookieRefreshToken { get; set; }
        public string ChaveCacheAcesso { get; set; }
        public bool CacheAcessoEncontrado { get; set; }
        public string Cache { get; set; }
        public string CacheProviderConfigurado { get; set; }
        public string CacheDiretorioArquivoJson { get; set; }
        public bool CacheDistribuido { get; set; }
        public DateTime HorarioServidorUtc { get; set; }
        public DateTime HorarioServidorLocal { get; set; }
    }

    internal class DiagnosticoCookiePayload
    {
        public string Valor { get; set; }
        public DateTime CriadoEmUtc { get; set; }
        public DateTime ExpiraEmUtc { get; set; }
    }
}
