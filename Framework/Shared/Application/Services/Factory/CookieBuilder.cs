using Microsoft.AspNetCore.Http;

namespace Shared.Application.Services.Factory
{
    public class CookieBuilder
    {
        private readonly IResponseCookies _cookies;

        public CookieBuilder(IResponseCookies cookies)
        {
            _cookies = cookies;
        }

        protected void MontarCookie(string chaveDoCookie, string dado)
        {
            try
            {
                _cookies.Append(chaveDoCookie, dado, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddDays(31) // Expandi para 31 por padrão o token de acesso difere desse tempo
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected void MontarCookie(string chaveDoCookie, string dado, DateTime expiracao)
        {
            _cookies.Append(chaveDoCookie, dado, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true,
                Expires = expiracao
            });
        }

        protected void RemoverCookie(string chaveDoCookie)
        {
            _cookies.Delete(chaveDoCookie, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = true
            });
        }
    }
}