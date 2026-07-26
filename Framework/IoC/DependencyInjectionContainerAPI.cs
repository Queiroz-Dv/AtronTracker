using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Services;
using Application.Services.AuthServices;
using Application.Services.EntitiesServices;
using Application.Services.EntitiesServices.PerfisDeAcesso;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.TarefaCases;
using Application.UseCases.UsuarioCases;
using Application.Validador;
using AtronNotificacoes.Client;
using AtronNotificacoes.Contracts;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.ApplicationRepositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities.Identity;
using System;
using System.IO;
using System.Net.Http;
using System.Text.Json.Serialization;

namespace IoC
{
    public static class DependencyInjectionContainerAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

            var database = DatabaseProviderResolver.Resolve(configuration);
            var migrationsAssembly = typeof(AtronDbContext).Assembly.GetName().Name!;

            services.AddDbContext<AtronDbContext>(options =>
                options.UseConfiguredDatabase(database, migrationsAssembly));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<AtronDbContext>()
                    .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromHours(24);
            });

            services = services.AddSharedInfrastructure(configuration);

            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddScoped(provider => provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Response.Cookies);

            services = services.AddDependencyInjectionApiDoc();
            services = services.AddServiceMappings();
            services = services.AddMessageValidationServices();
            services = services.AddInfrastructureSecurity(configuration);
            services = services.AddEmailServices(configuration);
            ConfigureNotificacoesTransversais(services, configuration);
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
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILoginRepository, LoginRepository>();

            services.AddScoped(provider => new CadastroUsuarioContext(
                provider.GetRequiredService<IUsuarioRepository>(),
                provider.GetRequiredService<IPerfilDeAcessoUsuarioRepository>(),
                provider.GetRequiredService<IPerfilDeAcessoRepository>(),
                provider.GetRequiredService<IUsuarioIdentityRepository>(),
                provider.GetRequiredService<IEmailService>(),
                provider.GetRequiredService<IAcessoEmailCompositor>(),
                provider.GetRequiredService<IValidador<UsuarioRegistroRequest>>(),
                provider.GetRequiredService<IHttpContextAccessor>(),
                provider.GetRequiredService<IConfirmacaoEmailRepository>(),
                provider.GetRequiredService<IConfirmacaoEmailCodigoService>()));

            services.AddScoped(provider => new RecuperacaoSenhaContext(
                provider.GetRequiredService<IUsuarioRepository>(),
                provider.GetRequiredService<IUsuarioIdentityRepository>(),
                provider.GetRequiredService<ILoginRepository>(),
                provider.GetRequiredService<ICacheService>(),
                provider.GetRequiredService<IEmailService>(),
                provider.GetRequiredService<IAcessoEmailCompositor>(),
                provider.GetRequiredService<IHttpContextAccessor>()));

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
            services.AddScoped<ITarefaPreparacaoService, TarefaPreparacaoService>();
            services.AddScoped<ITarefaObtencaoValidador, TarefaObtencaoValidador>();
            services.AddScoped<IAprovadorObtencaoTarefaResolver, AprovadorObtencaoTarefaResolver>();
            services.AddScoped<ISolicitacaoObtencaoTarefaMapeador, SolicitacaoObtencaoTarefaMapeador>();
            services.AddScoped<ITarefaNotificacaoInternaService, TarefaNotificacaoInternaService>();
            services.AddScoped<ITarefaUsuarioAtualService, TarefaUsuarioAtualService>();
            services.AddScoped<ITarefaConfiguracoesService, TarefaConfiguracoesService>();
            services.AddScoped<ITarefaObtencaoService, TarefaObtencaoService>();
            services.AddScoped<ITarefaMovimentacaoService, TarefaMovimentacaoService>();
            services.AddScoped<ITarefaEmailCompositor, TarefaEmailCompositor>();
            services.AddScoped<ITarefaNotificacaoService, TarefaNotificacaoService>();
            services.AddScoped<CriarTarefa>();
            services.AddScoped<ITarefaService, TarefaService>();
        }

        private static void ConfigureNotificacoesTransversais(IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient("AtronNotificacoes");

            if (!Uri.TryCreate(configuration["NotificacoesInternas:BaseUrl"], UriKind.Absolute, out var baseAddress))
            {
                services.AddScoped<INotificacoesInternasPublisher, NotificacoesInternasPublisherIndisponivel>();
                services.AddScoped<INotificacoesInternasConsultaClient, NotificacoesInternasConsultaIndisponivel>();
                return;
            }

            var baseAddressNormalizado = new Uri($"{baseAddress.ToString().TrimEnd('/')}/");

            services.AddScoped<INotificacoesInternasConsultaClient>(provider =>
                new NotificacoesInternasHttpConsultaClient(
                    provider.GetRequiredService<IHttpClientFactory>().CreateClient("AtronNotificacoes"),
                    baseAddressNormalizado));

            var secretKey = configuration["NotificacoesInternas:Servico:SecretKey"];
            var issuer = configuration["NotificacoesInternas:Servico:Issuer"];
            var audience = configuration["NotificacoesInternas:Servico:Audience"];
            var nomeDoServico = configuration["NotificacoesInternas:Servico:Nome"];

            if (string.IsNullOrWhiteSpace(secretKey) || string.IsNullOrWhiteSpace(issuer) ||
                string.IsNullOrWhiteSpace(audience) || string.IsNullOrWhiteSpace(nomeDoServico))
            {
                services.AddScoped<INotificacoesInternasPublisher, NotificacoesInternasPublisherIndisponivel>();
                return;
            }

            var options = new NotificacoesInternasServiceOptions(
                baseAddressNormalizado,
                issuer,
                audience,
                secretKey,
                nomeDoServico);
            services.AddScoped<INotificacoesInternasPublisher>(provider =>
                new NotificacoesInternasHttpPublisher(
                    provider.GetRequiredService<IHttpClientFactory>().CreateClient("AtronNotificacoes"),
                    options));
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<CriarUsuario>();
            services.AddScoped<AtualizarUsuario>();
            services.AddScoped<RemoverUsuario>();
            services.AddScoped<DesativarUsuario>();
            services.AddScoped<SolicitarReativacao>();
            services.AddScoped<ReativarUsuario>();
            services.AddScoped<ObterUsuario>();
            services.AddScoped<AlterarEmail>();
            services.AddScoped<ConfirmarAlteracaoEmail>();
            services.AddScoped<ReenviarConfirmacaoEmail>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IConfirmacaoEmailRepository, ConfirmacaoEmailRepository>();
            services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();
            services.AddScoped<IValidador<UsuarioRequest>, UsuarioRequestValidador>();
            services.AddScoped<IAsyncMap<UsuarioRequest, Usuario>, UsuarioRequestMapping>();
        }

        private static void ConfigureCargoServices(IServiceCollection services)
        {
            services.AddScoped<ICargoRepository, CargoRepository>();
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
