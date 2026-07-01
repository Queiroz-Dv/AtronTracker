using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Extensions;
using System.Text.Json;

namespace Shared.Application.Services.Factory
{
    public class CookieFactory : CookieBuilder, ICookieFactoryService
    {
        private readonly IDataProtector protector;
        public CookieFactory(IResponseCookies responseCookies, IDataProtectionProvider provider) : base(responseCookies)
        {
            protector = provider.CreateProtector("TokenCookieProtector");
        }

        private string TokenUsuarioCookie(string codigoUsuario, ETokenInfo tokenInfo) => $"{codigoUsuario}{tokenInfo.GetDescription()}".ToUpper();

        public void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken, string codigoUsuario)
        {
            var dadosCookie = new DadosDoRefreshTokenCookieDTO
            {
                UsuarioCodigo = codigoUsuario,
                RefreshToken = dadosDoRefreshToken.Token,
                Expires = dadosDoRefreshToken.Expires
            };

            var json = JsonSerializer.Serialize(dadosCookie);
            var jsonProtegido = protector.Protect(json);

            MontarCookie(TokenUsuarioCookie(codigoUsuario, ETokenInfo.RefreshToken), jsonProtegido, dadosDoRefreshToken.Expires);
        }

        public async Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest request)
        {
            var codigo = request.Headers.ExtrairCodigoUsuarioDoRequest();
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            if (!request.Cookies.TryGetValue(TokenUsuarioCookie(codigo, ETokenInfo.RefreshToken), out var valor))
            {
                return null;
            }

            DadosDoRefreshTokenCookieDTO dados;
            try
            {
                var json = protector.Unprotect(valor);
                dados = JsonSerializer.Deserialize<DadosDoRefreshTokenCookieDTO>(json);
            }
            catch
            {
                return null;
            }

            if (dados is null || !string.Equals(dados.UsuarioCodigo, codigo, StringComparison.OrdinalIgnoreCase))
                return null;

            return await Task.FromResult(dados);
        }

        void ICookieFactoryService.RemoverCookie(string chave)
        {
            RemoverCookie(chave.ToUpper());
        }
    }
}
