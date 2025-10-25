using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Shared.DTO.API;
using Shared.Enums;
using Shared.Extensions;
using Shared.Interfaces.Factory;
using System.Text.Json;

namespace Shared.Services.Factory
{
    public class CookieFactory : CookieBuilder, ICookieFactory
    {
        private readonly IDataProtector protector;
        public CookieFactory(IResponseCookies responseCookies, IDataProtectionProvider provider) : base(responseCookies)
        {
            protector = provider.CreateProtector("TokenCookieProtector");
        }

        private string TokenUsuarioCookie(string codigoUsuario, ETokenInfo tokenInfo) => $"{codigoUsuario}{tokenInfo.GetDescription()}".ToUpper();

        public void CriarCookieDoToken(DadosDoTokenDTO dadosDoToken, string codigoUsuario)
        {
            var json = JsonSerializer.Serialize(dadosDoToken);
            var jsonProtegido = protector.Protect(json);

            MontarCookie(TokenUsuarioCookie(codigoUsuario, ETokenInfo.AcesssToken), jsonProtegido);
        }

        public async Task<DadosDoTokenDTO> ObterDadosDoTokenPorRequest(HttpRequest request)
        {
            var codigo = request.Headers.ExtrairCodigoUsuarioDoRequest();
            if (string.IsNullOrWhiteSpace(codigo)) return null;

            if (!request.Cookies.TryGetValue(TokenUsuarioCookie(codigo, ETokenInfo.AcesssToken), out var valor)) return null;

            var json = protector.Unprotect(valor);
            var dados = JsonSerializer.Deserialize<DadosDoTokenDTO>(json);

            return await Task.FromResult(dados);
        }

        void ICookieFactory.RemoverCookie(string chave)
        {
            RemoverCookie(chave.ToUpper());
        }
    }
}