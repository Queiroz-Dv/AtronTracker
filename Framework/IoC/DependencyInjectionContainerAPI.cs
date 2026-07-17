using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Services;
using Application.Services.AuthServices;
using Application.Services.EntitiesServices;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.Usuario;
using Application.Validador;
using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Infrastructure.Repositories;
using Infrastructure.Repositories.ApplicationRepositories;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Entities.Identity;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace IoC
{
    public static class DependencyInjectionContainerAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.TryAddSingleton<IAtronConnectionStringProvider, AtronConnectionStringProvider>();

            var database = DatabaseProviderResolver.Resolve(configuration);
            var migrationsAssembly = "AtronTracker.Infrastructure.PostgreSqlMigrations";

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
            ConfigureModuloServices(services);
            ConfigureTarefaServices(services);
            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigurePlanejamentoCustoServices(services);
            ConfigureUsuarioServices(services);
            ConfigureUsuarioCargoDepartamentoServices(services);
            ConfigureTarefaRepositoryServices(services);
            ConfigureNotificacaoInternaServices(services);
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
            services.AddScoped<ISolicitacaoObtencaoTarefaRepository, SolicitacaoObtencaoTarefaRepository>();
            services.AddScoped<ITarefaEstadoRepository, TarefaEstadoRepository>();
            services.AddScoped<ITarefaPreparacaoService, TarefaPreparacaoService>();
            services.AddScoped<ITarefaEmailCompositor, TarefaEmailCompositor>();
            services.AddScoped<ITarefaNotificacaoService, TarefaNotificacaoService>();
            services.AddScoped<ITarefaService, TarefaService>();
        }

        private static void ConfigureNotificacaoInternaServices(IServiceCollection services)
        {
            services.AddScoped<INotificacaoInternaRepository, NotificacaoInternaRepository>();
            services.AddScoped<INotificacaoInternaService, NotificacaoInternaService>();
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
            services.AddScoped<IPerfilDeAcessoService, PerfilDeAcessoService>();
        }

        private static void ConfigureTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();
        }
    }
}
