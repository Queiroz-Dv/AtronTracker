using AtronNotificacoes.Application;
using AtronNotificacoes.Contracts;
using AtronNotificacoes.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Infrastructure.Configuration;

namespace AtronNotificacoes.Infrastructure.DependencyInjection;

public static class NotificacoesInternasServiceCollectionExtensions
{
    public const string TagProntidao = "notificacoes-ready";

    public static IServiceCollection AddNotificacoesInternasCapability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

        var database = DatabaseProviderResolver.Resolve(configuration);
        var migrationsAssembly = typeof(NotificacoesDbContext).Assembly.GetName().Name!;

        services.AddDbContext<NotificacoesDbContext>(options =>
            options.UseConfiguredDatabase(database, migrationsAssembly));
        services.AddScoped<INotificacaoInternaRepository, NotificacaoInternaRepository>();
        services.AddScoped<INotificacaoInternaService, NotificacaoInternaService>();
        services.AddScoped<INotificacoesInternasPublisher, NotificacoesInternasInProcessPublisher>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(SegurancaNotificacoes.PoliticaUsuario, policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim(SegurancaNotificacoes.ClaimCodigoUsuario));
        });

        services.AddHealthChecks()
            .AddCheck<ProntidaoBancoNotificacoesCheck>(
                "banco-notificacoes",
                tags: [TagProntidao]);

        return services;
    }
}
