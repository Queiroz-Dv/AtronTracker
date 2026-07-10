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

        protected void MontarCookie(string chaveDoCookie, string dado, DateTime expiracao)
        {
            _cookies.Append(chaveDoCookie, dado, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = expiracao
            });
        }

        protected void RemoverCookie(string chaveDoCookie)
        {
            _cookies.Delete(chaveDoCookie, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                Secure = true
            });
        }
    }
}
