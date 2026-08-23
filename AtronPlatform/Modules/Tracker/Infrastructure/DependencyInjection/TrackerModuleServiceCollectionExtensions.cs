using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Contexts;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Identity;
using Application.Mapping;
using Application.Policies.PlanejamentoCustos;
using Application.Policies.Tarefas;
using Application.Resolvers.Tarefas;
using Application.Records.Usuario;
using Application.Services.AuthServices;
using Application.Services.Contexts;
using Application.Services.EntitiesServices;
using Application.Services.EntitiesServices.PerfisDeAcesso;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Application.Services.EntitiesServices.Tarefas;
using Application.Services.EntitiesServices.Tarefas.Obtencao;
using Application.Services.Identity;
using Application.UseCases.CargoCases;
using Application.UseCases.DepartamentoCases;
using Application.UseCases.PerfilDeAcessoCases;
using Application.UseCases.TarefaCases;
using Application.UseCases.TarefaCases.Movimentacao;
using Application.UseCases.UsuarioCases;
using Application.Validador;
using AtronTracker.Infrastructure.Context;
using AtronTracker.Infrastructure.Identity;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Infrastructure.Configuration;
using Infrastructure.Repositories;
using Infrastructure.Repositories.ApplicationRepositories;
using Infrastructure.Repositories.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities.Identity;
using Shared.Infrastructure.Configuration;

namespace Infrastructure.DependencyInjection
{
    public static class TrackerModuleServiceCollectionExtensions
    {
        public static IServiceCollection AddTrackerModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

            var database = DatabaseProviderResolver.Resolve(configuration);
            var migrationsAssembly = typeof(AtronDbContext).Assembly.GetName().Name!;

            services.AddDbContext<AtronDbContext>(options =>
                options.UseConfiguredDatabase(database, migrationsAssembly));

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                    {
                        options.Lockout.AllowedForNewUsers = true;
                        options.Lockout.MaxFailedAccessAttempts = 5;
                        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    })
                    .AddEntityFrameworkStores<AtronDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            services.AddTrackerSharedAdapters();
            services.AddTrackerMappings();
            services.AddTrackerValidations();
            services.AddTrackerAuthorization();
            ConfigureModuloServices(services);
            ConfigureTarefaServices(services);
            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigurePlanejamentoCustoServices(services);
            ConfigureUsuarioServices(services);
            ConfigureUsuarioCargoDepartamentoServices(services);
            ConfigureTarefaRepositoryServices(services);
            ConfigureDefaultUserRoleServices(services);
            ConfigureAuthenticationServices(services);
            ConfigurePerfilDeAcessoServices(services);
            ConfigurePerfilDeAcessoUsuarioServices(services);


            services.AddDataProtection()
                .SetApplicationName("Atron")
                .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))
                .SetDefaultKeyLifetime(TimeSpan.FromDays(90));
            return services;
        }

        private static void AddTrackerSharedAdapters(this IServiceCollection services)
        {
            services.AddScoped<ILoginContext, LoginContext>();
            services.AddScoped<IUsuarioContext, UsuarioContext>();
            services.AddScoped<IControleDeSessaoContext, ControleDeSessaoContext>();
            services.AddScoped<ICacheUsuarioService, CacheUsuarioService>();
            services.AddScoped<IDadosComplementaresDoUsuarioService, DadosComplementaresDoUsuarioService>();
            services.AddScoped<IUserIdentityService, UserIdentityService>();
            services.AddScoped<IUsuarioIdentityRepository, UserIdentityRepository>();
            services.AddScoped<IRefreshTokenUnicidadeService, RefreshTokenUnicidadeService>();
        }

        private static void ConfigurePerfilDeAcessoUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IPerfilDeAcessoUsuarioRepository, PerfilDeAcessoUsuarioRepository>();
        }

        private static void ConfigureUsuarioCargoDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioCargoDepartamentoRepository, UsuarioCargoDepartamentoRepository>();
        }

        private static void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services.AddSingleton<IEnderecoFrontendService, EnderecoFrontendService>();
            services.AddSingleton<ITokenTemporarioService, TokenTemporarioService>();
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILoginRepository, LoginRepository>();

            services.AddScoped(provider => new CadastroUsuarioContextRecord(
                provider.GetRequiredService<IUsuarioRepository>(),
                provider.GetRequiredService<IUsuarioIdentityRepository>(),
                provider.GetRequiredService<IEmailService>(),
                provider.GetRequiredService<IAcessoEmailCompositor>(),
                provider.GetRequiredService<IValidador<UsuarioRegistroRequest>>(),
                provider.GetRequiredService<IEnderecoFrontendService>(),
                provider.GetRequiredService<IConfirmacaoEmailRepository>(),
                provider.GetRequiredService<IConfirmacaoEmailCodigoService>()));

            services.AddScoped(provider => new RecuperacaoSenhaContextRecord(
                provider.GetRequiredService<IUsuarioRepository>(),
                provider.GetRequiredService<IUsuarioIdentityRepository>(),
                provider.GetRequiredService<ILoginRepository>(),
                provider.GetRequiredService<ICacheService>(),
                provider.GetRequiredService<IEmailService>(),
                provider.GetRequiredService<IAcessoEmailCompositor>(),
                provider.GetRequiredService<IEnderecoFrontendService>(),
                provider.GetRequiredService<ITokenTemporarioService>()));

            services.AddScoped<ICadastroUsuarioService, CadastroUsuarioService>();
            services.AddScoped<IRecuperacaoSenhaService, RecuperacaoSenhaService>();
            services.AddScoped<IRegistroUsuarioService, RegistroUsuarioService>();
            services.AddScoped<IAcessoEmailCompositor, AcessoEmailCompositor>();
            services.AddScoped<IConfirmacaoEmailCodigoService, ConfirmacaoEmailCodigoService>();
            services.AddScoped<IValidador<DadosDoTokenDTO>, DadosDoTokenValidador>();
            services.AddScoped<IValidador<UsuarioRegistroRequest>, UsuarioRegistroValidador>();
        }

        private static void ConfigureDefaultUserRoleServices(IServiceCollection services)
        {
            services.AddScoped<ICreateDefaultUserRoleRepository, CreateDefaultUserRoleRepository>();
        }

        private static void ConfigureTarefaRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<ITarefaMovimentacaoRepository, TarefaMovimentacaoRepository>();
            services.AddScoped<ISolicitacaoObtencaoTarefaRepository, SolicitacaoObtencaoTarefaRepository>();
            services.AddScoped<ITarefaEstadoRepository, TarefaEstadoRepository>();
            services.AddScoped<TarefaEstadoService>();
            services.AddScoped<TarefaUsuarioRelacionamentoService>();
            services.AddScoped<TarefaDepartamentoCargoRelacionamentoService>();
            services.AddScoped<TarefaRelacionamentoService>();
            services.AddScoped<ITarefaPreparacaoService, TarefaPreparacaoService>();
            services.AddScoped<ITarefaObtencaoPolicy, TarefaObtencaoPolicy>();
            services.AddScoped<AprovadorObtencaoTarefaResolver>();
            services.AddScoped<ITarefaConfiguracoesService, TarefaConfiguracoesService>();
            services.AddScoped<ITarefaObtencaoService, TarefaObtencaoService>();
            services.AddScoped<ITarefaEmailCompositor, TarefaEmailCompositor>();
            services.AddScoped<ITarefaNotificacaoService, TarefaNotificacaoService>();

            services.AddScoped<AssumirTarefaCase>();
            services.AddScoped<AtualizarTarefaCase>();
            services.AddScoped<AtualizarTarefaMovimentacaoCase>();
            services.AddScoped<CriarTarefaCase>();
            services.AddScoped<CriarTarefaMovimentacaoCase>();
            services.AddScoped<DecidirTarefaCase>();
            services.AddScoped<ExcluirTarefaCase>();
            services.AddScoped<ObterAcessoTarefaCase>();
            services.AddScoped<ObterEquipeCase>();
            services.AddScoped<ObterHistoricoTarefaCase>();
            services.AddScoped<ObterMeuQuadroCase>();
            services.AddScoped<ObterSolicitacaoCase>();
            services.AddScoped<ObterTarefasDisponiveisCase>();
            services.AddScoped<ObterTarefaCase>();
            services.AddScoped<RegistrarDecisaoTarefaMovimentacaoCase>();
            services.AddScoped<RegistrarObtencaoTarefaMovimentacaoCase>();
            services.AddScoped<RegistrarSolicitacaoTarefaMovimentacaoCase>();
            services.AddScoped<SolicitarTarefaCase>();
            services.AddScoped<TarefaNotificacaoInternaCase>();

            services.AddScoped<ITarefaService, TarefaService>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<AssociarUsuarioCargoDepartamentoCase>();
            services.AddScoped<AtualizarAssociacaoUsuarioCargoDepartamentoCase>();
            services.AddScoped<AtualizarCredenciaisUsuarioCase>();
            services.AddScoped<AuditoriaUsuarioCase>();
            services.AddScoped<CriarUsuarioCase>();
            services.AddScoped<AtualizarUsuarioCase>();
            services.AddScoped<EnviarEmailPrimeiroAcessoCase>();
            services.AddScoped<RemoverUsuarioCase>();
            services.AddScoped<DesativarUsuarioCase>();
            services.AddScoped<SolicitarReativacaoCase>();
            services.AddScoped<ReativarUsuarioCase>();
            services.AddScoped<ObterUsuarioCase>();
            services.AddScoped<AlterarEmailCase>();
            services.AddScoped<ConfirmarAlteracaoEmailCase>();
            services.AddScoped<ReenviarConfirmacaoEmailCase>();
            services.AddScoped<VerificarAtualizacaoUsuarioCase>();
            services.AddScoped<VerificarUsuarioCase>();
            services.AddScoped<VincularGestorImediatoCase>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IConfirmacaoEmailRepository, ConfirmacaoEmailRepository>();
            services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();
            services.AddScoped<IValidador<UsuarioRequest>, UsuarioRequestValidador>();
        }

        private static void ConfigureCargoServices(IServiceCollection services)
        {
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<AtualizarCargoCase>();
            services.AddScoped<CriarCargoCase>();
            services.AddScoped<ExcluirCargoCase>();
            services.AddScoped<ObterCargoCase>();
            services.AddScoped<ICargoService, CargoService>();
        }

        private static void ConfigurePlanejamentoCustoServices(IServiceCollection services)
        {
            services.AddScoped<IPlanejamentoCustoRepository, PlanejamentoCustoRepository>();
            services.AddScoped<EstruturaPlanejadaPolicy>();
            services.AddScoped<IPlanejamentoCustoPreparacaoService, PlanejamentoCustoPreparacaoService>();
            services.AddScoped<IPlanejamentoCustoRelatorioService, PlanejamentoCustoRelatorioService>();
            services.AddScoped<IPlanejamentoCustoRelatorioImpressaoService, PlanejamentoCustoRelatorioImpressaoService>();
            services.AddScoped<IPlanejamentoCustoService, PlanejamentoCustoService>();
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<AtualizarDepartamentoCase>();
            services.AddScoped<CriarDepartamentoCase>();
            services.AddScoped<ExcluirDepartamentoCase>();
            services.AddScoped<ObterDepartamentoCase>();
            services.AddScoped<VincularGestorDepartamentoCase>();
            services.AddScoped<IDepartamentoService, DepartamentoService>();
        }

        private static void ConfigureModuloServices(IServiceCollection services)
        {
            services.AddScoped<IModuloRepository, ModuloRepository>();
            services.AddScoped<IModuloService, ModuloService>();
        }

        private static void ConfigurePerfilDeAcessoServices(IServiceCollection services)
        {
            services.AddScoped<IPerfilDeAcessoRepository, PerfilDeAcessoRepository>();
            services.AddScoped<AtualizarPerfilDeAcessoCase>();
            services.AddScoped<CriarPerfilDeAcessoCase>();
            services.AddScoped<ObterPerfilDeAcessoCase>();
            services.AddScoped<ObterPerfisUsuarioCase>();
            services.AddScoped<RemoverPerfilDeAcessoCase>();
            services.AddScoped<IPerfilDeAcessoPreparacaoService, PerfilDeAcessoPreparacaoService>();
            services.AddScoped<IPerfilDeAcessoCacheInvalidator, PerfilDeAcessoCacheInvalidator>();
            services.AddScoped<IPerfilDeAcessoUsuarioRelacionamentoService, PerfilDeAcessoUsuarioRelacionamentoService>();
            services.AddScoped<IPerfilDeAcessoService, PerfilDeAcessoService>();
        }

        private static void ConfigureTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();
        }
    }
}
