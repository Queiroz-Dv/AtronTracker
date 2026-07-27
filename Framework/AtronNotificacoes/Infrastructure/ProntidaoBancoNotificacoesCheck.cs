using AtronNotificacoes.Resources;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AtronNotificacoes.Infrastructure;

public sealed class ProntidaoBancoNotificacoesCheck(NotificacoesDbContext databaseContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await databaseContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy(NotificacoesResource.Saude_BancoDisponivel)
                : HealthCheckResult.Unhealthy(NotificacoesResource.Saude_BancoIndisponivel);
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy(
                NotificacoesResource.Saude_FalhaBanco,
                exception);
        }
    }
}
