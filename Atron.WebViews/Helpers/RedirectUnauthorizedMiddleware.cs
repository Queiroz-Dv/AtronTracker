using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Atron.WebViews.Helpers
{
    public class RedirectUnauthorizedMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectUnauthorizedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.Redirect("/ApplicationLogin/Login");
            }
        }
    }
}