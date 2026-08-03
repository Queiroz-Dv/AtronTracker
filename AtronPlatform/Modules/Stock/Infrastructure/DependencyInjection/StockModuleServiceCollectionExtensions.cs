using AtronStock.Application.DTO.Request;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Mapping;
using AtronStock.Application.Services;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using AtronStock.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Configuration;

namespace AtronStock.Infrastructure;

public static class StockModuleServiceCollectionExtensions
{
    public static IServiceCollection AddStockModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

        var database = DatabaseProviderResolver.Resolve(configuration);
        var migrationsAssembly = typeof(StockDbContext).Assembly.GetName().Name!;

        services.AddDbContext<StockDbContext>(options =>
            options.UseConfiguredDatabase(database, migrationsAssembly));

        services.AddScoped<IClienteService, ClienteService>();
        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<ICategoriaService, CategoriaService>();
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IEstoqueService, EstoqueService>();
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();

        services.AddScoped<IResponsavelNotificacaoEstoqueResolver>(_ =>
            new ResponsavelNotificacaoEstoqueResolver(
                configuration["NotificacoesEstoque:ResponsavelCodigo"]));
        services.AddScoped<IEstoqueNotificacaoService, EstoqueNotificacaoService>();

        services.AddScoped<IAsyncMap<ClienteRequest, Cliente>, ClienteMapping>();
        services.AddScoped<IAsyncMap<CategoriaRequest, Categoria>, CategoriaMapping>();
        services.AddScoped<IAsyncMap<ProdutoRequest, Produto>, ProdutoMapping>();
        services.AddScoped<IAsyncMap<FornecedorRequest, Fornecedor>, FornecedorMapping>();

        services.AddScoped<IValidador<ClienteRequest>, ClienteValidador>();
        services.AddScoped<IValidador<CategoriaRequest>, CategoriaValidador>();
        services.AddScoped<IValidador<ProdutoRequest>, ProdutoValidador>();
        services.AddScoped<IValidador<FornecedorRequest>, FornecedorValidador>();

        return services;
    }
}
