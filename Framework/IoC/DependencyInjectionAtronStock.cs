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
using Shared.Infrastructure.Repositories;
using Shared.Repositories;
using System.Linq;

namespace IoC
{
    public static class DependencyInjectionAtronStock
    {
        public static IServiceCollection AddStockInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            string sqlConnection = configuration.GetConnectionString("AtronConnection");

            services.AddDbContext<StockDbContext>(options => options.UseSqlServer(sqlConnection, b => b.MigrationsAssembly(typeof(StockDbContext).Assembly.FullName)));
            services = services.AddSharedInfrastructure(configuration);
            services.AddScoped<ITransactionManager, TransactionManager>();

            var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(SharedDbContext));
            if (descriptor != null) services.Remove(descriptor);

            services.AddScoped(provider =>
            {
                var stockContext = provider.GetRequiredService<StockDbContext>();
                var connection = stockContext.Database.GetDbConnection();

                var optionsBuilder = new DbContextOptionsBuilder<SharedDbContext>();
                optionsBuilder.UseSqlServer(connection);

                return new SharedDbContext(optionsBuilder.Options);
            });

            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IClienteRepository, ClienteRepository>();

            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<ICategoriaRepository, CategoriaRepository>();

            services.AddScoped<IFornecedorRepository, FornecedorRepository>();
            services.AddScoped<IProdutoRepository, ProdutoRepository>();
            services.AddScoped<IEstoqueRepository, EstoqueRepository>();

            services.AddScoped<IProdutoService, ProdutoService>();
            services.AddScoped<IFornecedorService, FornecedorService>();
            services.AddScoped<IEstoqueService, EstoqueService>();

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
            services.AddScoped<IAsyncMap<CategoriaRequest, Categoria>, CategoriaMapping>();
            services.AddScoped<IAsyncMap<ProdutoRequest, Produto>, ProdutoMapping>();
            services.AddScoped<IAsyncMap<FornecedorRequest, Fornecedor>, FornecedorMapping>();
            return services;
        }
    }

    internal static class DependencyInjectionAtronStockValidador
    {
        public static IServiceCollection AddStockValidador(this IServiceCollection services)
        {
            services.AddScoped<IValidador<ClienteRequest>, ClienteValidador>();
            services.AddScoped<IValidador<CategoriaRequest>, CategoriaValidador>();
            services.AddScoped<IValidador<ProdutoRequest>, ProdutoValidador>();
            services.AddScoped<IValidador<FornecedorRequest>, FornecedorValidador>();
            return services;
        }
    }
}
