using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;

namespace Shared.Application.Services.Factory;

public class CookieFactory : CookieBuilder, ICookieFactoryService
{
    public const string NomeCookieRefreshToken = "ATRON_REFRESH_TOKEN";

    public CookieFactory(IResponseCookies responseCookies) : base(responseCookies) { }

    public void CriarCookieDeRefreshToken(DadosDoRefrehTokenDTO dadosDoRefreshToken)
    {
        MontarCookie(
            NomeCookieRefreshToken,
            dadosDoRefreshToken.Token,
            dadosDoRefreshToken.Expires);
    }

    public Task<DadosDoRefreshTokenCookieDTO> ObterRefreshTokenPorRequest(HttpRequest request)
    {
        if (!request.Cookies.TryGetValue(NomeCookieRefreshToken, out var refreshToken))
            return Task.FromResult<DadosDoRefreshTokenCookieDTO>(null);

        return Task.FromResult(new DadosDoRefreshTokenCookieDTO
        {
            RefreshToken = refreshToken
        });
    }

    public void RemoverCookieDeRefreshToken()
    {
        RemoverCookie(NomeCookieRefreshToken);
    }
}
