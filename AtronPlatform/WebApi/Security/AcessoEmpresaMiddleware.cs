using Microsoft.AspNetCore.Authorization;
using Shared.Application.Interfaces.Service;

namespace AtronPlatform.WebApi.Security;

public sealed class AcessoEmpresaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IEmpresaAtualService empresaAtual)
    {
        var endpoint = context.GetEndpoint();
        if (context.User.Identity?.IsAuthenticated != true
            || endpoint is null
            || endpoint.Metadata.GetMetadata<IAuthorizeData>() is null
            || endpoint.Metadata.GetMetadata<IAllowAnonymous>() is not null
            || endpoint.Metadata.GetMetadata<PermitirSemEmpresaAttribute>() is not null)
        {
            await next(context);
            return;
        }

        var empresa = await empresaAtual.ObterAsync();
        if (!empresa.AcessoPermitido)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new
            {
                codigo = "EMPRESA_ACESSO_BLOQUEADO",
                mensagem = empresa.MotivoBloqueio
            });
            return;
        }

        await next(context);
    }
}
