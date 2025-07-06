using Microsoft.AspNetCore.Http;

namespace Shared.Services.Factory
{
    public class CookieBuilder
    {
        private readonly IResponseCookies _cookies;

        public CookieBuilder(IResponseCookies cookies)
        {
            _cookies = cookies;
        }

        protected  void MontarCookie(string descricaoCookie, string dado)
        {
            _cookies.Append(descricaoCookie, dado, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = DateTime.UtcNow.AddMinutes(15) // Ajuste a expiração conforme necessário
            });
        }

        protected void MontarCookie(string descricaoCookie, string dado, DateTime expiracao)
        {
            _cookies.Append(descricaoCookie, dado, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = expiracao
            });
        }

        protected void RemoverCookie(string descricaoCookie)
        {
            _cookies.Delete(descricaoCookie, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
        }
    }
}