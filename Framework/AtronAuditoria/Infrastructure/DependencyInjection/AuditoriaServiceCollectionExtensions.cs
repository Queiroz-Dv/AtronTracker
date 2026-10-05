using AtronAuditoria.Repositories;
using Microsoft.EntityFrameworkCore; 
using Microsoft.Extensions.Configuration; 
using Microsoft.Extensions.DependencyInjection; 
using Microsoft.Extensions.DependencyInjection.Extensions; 
using AtronAuditoria.Application.Interfaces.Repositories; 
using Shared.Application.Services; 
using Shared.Application.Services.Accessor; 
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Configuration; 
using AtronAuditoria.Infrastructure.Context; 
using Shared.Repositories;
using Shared.Application.Messaging;
using Shared.Domain.Events.Auditoria;
using AtronAuditoria.Application.EventHandlers;
using AtronAuditoria.Application.UseCases;

namespace AtronAuditoria.Infrastructure.DependencyInjection 
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
            
            services.AddDbContext<AtronAuditoriaContext>(options =>                 
                options.UseConfiguredDatabase(database, migrationsAssembly));              
            
            services.AddHttpContextAccessor();             
            services.TryAddScoped<IUserAccessor, UserAccessor>();             
            
            services.TryAddScoped<ObterAuditoriaCase>();             
            services.TryAddScoped<IAuditoriaRepository, AuditoriaRepository>();             
            services.TryAddScoped<IHistoricoRepository, HistoricoRepository>();

            services.AddTransient<IEventHandler<AuditoriaRegistradaEvent>, AuditoriaEventHandlers>();
            services.AddTransient<IEventHandler<AuditoriaAtualizadaEvent>, AuditoriaEventHandlers>();
            services.AddTransient<IEventHandler<AuditoriaRemovidaEvent>, AuditoriaEventHandlers>();              
            
            return services;         
        }     
    } 
}
