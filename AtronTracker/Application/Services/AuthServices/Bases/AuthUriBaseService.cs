using Microsoft.AspNetCore.Http;

namespace Application.Services.AuthServices.Bases
{
    public abstract class AuthUriBaseService(IHttpContextAccessor httpContextAccessor)
    {
        protected string ObterUri(string clientUri)
        {
            if (!string.IsNullOrEmpty(clientUri))
                return clientUri;

            var request = httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
