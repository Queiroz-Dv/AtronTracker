namespace AtronPlatform.WebApi.Security;

public sealed class CabecalhosSegurancaMiddleware(RequestDelegate next)
{
    private const string ContentSecurityPolicy =
        "default-src 'none'; base-uri 'none'; frame-ancestors 'none'; form-action 'none'";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers.ContentSecurityPolicy = ContentSecurityPolicy;
            headers.XFrameOptions = "DENY";
            headers.XContentTypeOptions = "nosniff";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=(), payment=(), usb=()";
            return Task.CompletedTask;
        });

        await next(context);
    }
}

public static class CabecalhosSegurancaApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCabecalhosSeguranca(this IApplicationBuilder app)
        => app.UseMiddleware<CabecalhosSegurancaMiddleware>();
}
