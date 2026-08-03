using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services;
using Shared.Application.Services.Accessor;
using Shared.Infrastructure.Configuration;
using Shared.Infrastructure.Context;
using Shared.Repositories;

namespace Shared.Infrastructure.DependencyInjection
{
    public static class AuditoriaServiceCollectionExtensions
    {
        public static IServiceCollection AddAuditoriaCapability(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

            var database = DatabaseProviderResolver.Resolve(configuration);
            const string migrationsAssembly = "Framework.Shared.Migrations";

            services.AddDbContext<SharedDbContext>(options =>
                options.UseConfiguredDatabase(database, migrationsAssembly));

            services.AddHttpContextAccessor();
            services.TryAddScoped<IUserAccessor, UserAccessor>();
            services.TryAddScoped<IAuditoriaService, AuditoriaService>();
            services.TryAddScoped<IHistoricoService, HistoricoService>();
            services.TryAddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.TryAddScoped<IHistoricoRepository, HistoricoRepository>();

            return services;
        }
    }
}
