using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services;
using Shared.Application.Services.Accessor;
using Shared.Application.Services.Contexts;
using Shared.Application.Services.Factory;
using Shared.Infrastructure.Repositories;
using Shared.Repositories;

namespace Shared.Infrastructure.DependencyInjection
{
    public static class SharedInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuditoriaCapability(configuration);

            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ITransactionManager, TransactionManager>();
            services.AddScoped<IAccessorService, ServiceAccessor>();
            services.AddScoped<ICookieService, CookieService>();
            services.AddScoped<ICookieFactoryService, CookieFactory>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<ITokenFactoryService, TokenFactory>();
            services.AddScoped<IAuthManagerService, AuthManagerContext>();
            return services;
        }
    }
}
