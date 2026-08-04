using Microsoft.AspNetCore.RateLimiting;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using System.Globalization;
using System.Threading.RateLimiting;

namespace AtronPlatform.WebApi.Security;

public static class AcessoRateLimiting
{
    public const string Login = "acesso-login";
    public const string RecuperacaoSenha = "acesso-recuperacao-senha";
    public const string ReenvioConfirmacao = "acesso-reenvio-confirmacao";
    public const string ConfirmacaoEmail = "acesso-confirmacao-email";
    public const string Registro = "acesso-registro";
    public const string TrocaSenha = "acesso-troca-senha";
    public const string Reativacao = "acesso-reativacao";

    public static IServiceCollection AddAcessoRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            AdicionarPolitica(options, new(Login, 5, TimeSpan.FromMinutes(1)));
            AdicionarPolitica(options, new(RecuperacaoSenha, 3, TimeSpan.FromMinutes(10)));
            AdicionarPolitica(options, new(ReenvioConfirmacao, 3, TimeSpan.FromMinutes(10)));
            AdicionarPolitica(options, new(ConfirmacaoEmail, 5, TimeSpan.FromMinutes(10)));
            AdicionarPolitica(options, new(Registro, 3, TimeSpan.FromHours(1)));
            AdicionarPolitica(options, new(TrocaSenha, 5, TimeSpan.FromMinutes(10)));
            AdicionarPolitica(options, new(Reativacao, 3, TimeSpan.FromMinutes(10)));

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, cancellationToken) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = Math.Ceiling(retryAfter.TotalSeconds)
                        .ToString(CultureInfo.InvariantCulture);
                }

                var mensagem = new NotificationMessage
                {
                    Descricao = AuthResource.Erro_MuitasTentativas,
                    Nivel = ENotificationType.Error
                };

                await context.HttpContext.Response.WriteAsJsonAsync(
                    new[] { mensagem },
                    cancellationToken);
            };
        });

        return services;
    }

    private static void AdicionarPolitica(
        RateLimiterOptions options,
        PoliticaRateLimit politica)
    {
        options.AddPolicy(politica.Nome, contexto =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: contexto.Connection.RemoteIpAddress?.ToString() ?? "ip-desconhecido",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = politica.Limite,
                    Window = politica.Janela,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));
    }

    private sealed record PoliticaRateLimit(string Nome, int Limite, TimeSpan Janela);
}