using AtronStock.Application.DTO.Request;
using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Mapping;
using AtronStock.Application.Providers.Notificacoes;
using AtronStock.Application.Services;
using AtronStock.Application.UseCases.CategoriaCases;
using AtronStock.Application.UseCases.ProdutoCases;
using AtronStock.Application.Validacoes;
using AtronStock.Domain.Entities;
using AtronStock.Domain.Interfaces;
using AtronStock.Infrastructure.Context;
using AtronStock.Infrastructure.Repositories;
using AtronStock.Infrastructure.Workers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.Interfaces.Mapping;
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
        ConfigureCategoriaServices(services);
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IFornecedorRepository, FornecedorRepository>();
        ConfigureProdutoServices(services);
        if (configuration.GetValue("ProcessamentosProdutosLote:WorkerHabilitado", true))
            services.AddHostedService<GeracaoProdutosLoteWorker>();
        services.AddScoped<IEstoqueService, EstoqueService>();
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();

        services.AddScoped<ResponsavelNotificacaoEstoqueProvider>(_ =>
            new ResponsavelNotificacaoEstoqueProvider(
                configuration["NotificacoesEstoque:ResponsavelCodigo"]));
        services.AddScoped<IEstoqueNotificacaoService, EstoqueNotificacaoService>();

        services.AddMapper<Cliente, ClienteRequest, ClienteMapping>();
        services.AddMapper<Categoria, CategoriaRequest, CategoriaMapping>();
        services.AddMapper<Fornecedor, FornecedorRequest, FornecedorMapping>();

        services.AddScoped<IValidador<ClienteRequest>, ClienteValidador>();
        services.AddScoped<IValidador<CategoriaRequest>, CategoriaValidador>();
        services.AddScoped<IValidador<FornecedorRequest>, FornecedorValidador>();

        return services;
    }

    private static void ConfigureCategoriaServices(IServiceCollection services)
    {
        services.AddScoped<ICategoriaRepository, CategoriaRepository>();
        services.AddScoped<AuditoriaCategoriaCase>();
        services.AddScoped<AtualizarCategoriaCase>();
        services.AddScoped<AtivarInativarCategoriaCase>();
        services.AddScoped<CriarCategoriaCase>();
        services.AddScoped<ExcluirCategoriaCase>();
        services.AddScoped<ObterCategoriaCase>();
        services.AddScoped<ICategoriaService, CategoriaService>();
    }

    private static void ConfigureProdutoServices(IServiceCollection services)
    {
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<ILoteProdutoRepository, LoteProdutoRepository>();
        services.AddScoped<IProcessamentoProdutoLoteRepository,
            ProcessamentoProdutoLoteRepository>();
        services.AddScoped<ProdutoMapping>();
        services.AddScoped<IToEntityMapper<Produto, ProdutoCriacaoMappingInput>>(provider =>
            provider.GetRequiredService<ProdutoMapping>());
        services.AddScoped<IToDtoMapper<Produto, ProdutoResponse>>(provider =>
            provider.GetRequiredService<ProdutoMapping>());
        services.AddScoped<IUpdateMapper<Produto, ProdutoAtualizacaoMappingInput>>(provider =>
            provider.GetRequiredService<ProdutoMapping>());
        services.AddScoped<ProdutoValidador>();
        services.AddScoped<IValidador<ProdutoRequest>>(provider =>
            provider.GetRequiredService<ProdutoValidador>());
        services.AddScoped<IValidador<ProdutoAtualizacaoRequest>>(provider =>
            provider.GetRequiredService<ProdutoValidador>());
        services.AddScoped<GeracaoProdutosLoteValidador>();
        services.AddScoped<AuditoriaProdutoCase>();
        services.AddScoped<SelecionarCategoriasProdutoCase>();
        services.AddScoped<CriarProdutoCase>();
        services.AddScoped<CriarLoteParaPersistenciaCase>();
        services.AddScoped<AtualizarProdutoCase>();
        services.AddScoped<ObterProdutoCase>();
        services.AddScoped<ExecutarGeracaoProdutosLoteCase>();
        services.AddScoped<SolicitarGeracaoProdutosLoteCase>();
        services.AddScoped<ProcessadorProdutosLote>();
        services.AddScoped<ProcessamentoProdutoMapping>();
        services.AddScoped<ObterMeusProcessamentosProdutoCase>();
        services.AddScoped<ObterProcessamentoProdutoCase>();
        services.AddScoped<IProcessamentoProdutoService, ProcessamentoProdutoService>();
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<IProdutoService, ProdutoService>();
    }

    private static void AddMapper<TEntity, TDto, TMapper>(this IServiceCollection services)
        where TEntity : class
        where TDto : class
        where TMapper : class, IMapper<TEntity, TDto>
    {
        services.AddScoped<TMapper>();
        services.AddScoped<IMapper<TEntity, TDto>>(provider =>
            provider.GetRequiredService<TMapper>());
        services.AddScoped<IToDtoMapper<TEntity, TDto>>(provider =>
            provider.GetRequiredService<TMapper>());
        services.AddScoped<IToEntityMapper<TEntity, TDto>>(provider =>
            provider.GetRequiredService<TMapper>());
    }
}
