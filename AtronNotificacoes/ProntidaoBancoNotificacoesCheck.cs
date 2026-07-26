using AtronNotificacoes.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AtronNotificacoes;

public sealed class ProntidaoBancoNotificacoesCheck(NotificacoesDbContext databaseContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await databaseContext.Database.CanConnectAsync(cancellationToken)
                ? HealthCheckResult.Healthy("Banco de notificações disponível.")
                : HealthCheckResult.Unhealthy("Banco de notificações indisponível.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("Falha ao verificar o banco de notificações.", exception);
        }
    }
}
