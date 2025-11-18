using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Mapping;
using AtronStock.Application.Services;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using AtronStock.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Context;

namespace IoC
{
    public static class DependencyInjectionAtronStock
    {
        public static IServiceCollection AddStockInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string sqlConnection = configuration.GetConnectionString("AtronConnection");

            services.AddDbContext<StockDbContext>(options =>
            options.UseSqlServer(sqlConnection,
            b => b.MigrationsAssembly(typeof(StockDbContext).Assembly.FullName)));
            
            services = services.AddSharedInfrastructure(configuration);

            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();
            services = services.AddStockMapping();
            services = services.AddStockValidador();
            return services;
        }
    }

    internal static class DependencyInjectionAtronStockMapping
    {
        public static IServiceCollection AddStockMapping(this IServiceCollection services)
        {
            services.AddScoped<IAsyncMap<ClienteRequest, Cliente>, ClienteMapping>();
            return services;
        }
    }

    internal static class DependencyInjectionAtronStockValidador
    {
        public static IServiceCollection AddStockValidador(this IServiceCollection services)
        {
            services.AddScoped<IValidador<ClienteRequest>, ClienteValidador>();
            return services;
        }
    }

    public static class DependencyInjectionAtronShared
    {
        public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string sqlConnection = configuration.GetConnectionString("AtronConnection");

            services.AddDbContext<SharedDbContext>(options =>
            options.UseSqlServer(sqlConnection,
            b => b.MigrationsAssembly(typeof(SharedDbContext).Assembly.FullName)));

            return services;
        }
    }
}
