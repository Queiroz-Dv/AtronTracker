using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Services.Contexts;
using Application.Services.EntitiesServices;
using Application.Services.Identity;
using Domain.Interfaces.Identity;
using Infrastructure.Repositories.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.Interfaces.Repositories;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services;
using Shared.Application.Services.Accessor;
using Shared.Application.Services.Contexts;
using Shared.Application.Services.Factory;
using Shared.Infrastructure.Context;
using Shared.Infrastructure.Repositories;
using Shared.Repositories;

namespace IoC
{
    public static class DependencyInjectionAtronShared
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

            var database = DatabaseProviderResolver.Resolve(configuration);
            var migrationsAssembly = "Framework.Shared.Migrations";

            services.AddDbContext<SharedDbContext>(options =>
                options.UseConfiguredDatabase(database, migrationsAssembly));

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IAccessorService, ServiceAccessor>();
            services.AddScoped<ILoginContext, LoginContext>();
            services.AddScoped<IUsuarioContext, UsuarioContext>();
            services.AddScoped<IControleDeSessaoContext, ControleDeSessaoContext>();

            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<ICookieFactoryService, CookieFactory>();

            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenFactoryService, TokenFactory>();

            services.AddScoped<ICacheUsuarioService, CacheUsuarioService>();
            services.AddScoped<IDadosComplementaresDoUsuarioService, DadosComplementaresDoUsuarioService>();

            services.AddScoped<IAuthManagerService, AuthManagerContext>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IUsuarioIdentityRepository, UserIdentityRepository>();

            services.AddScoped<IUserAccessor, UserAccessor>();

            services.AddScoped<IAuditoriaService, AuditoriaService>();
            services.AddScoped<IHistoricoService, HistoricoService>();

            services.AddScoped<IAuditoriaRepository, AuditoriaRepository>();
            services.AddScoped<IHistoricoRepository, HistoricoRepository>();
            return services;
        }
    }
}
