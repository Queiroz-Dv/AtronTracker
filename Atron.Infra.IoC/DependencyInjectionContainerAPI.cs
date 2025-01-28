using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices.ApplicationServices;
using Atron.Application.Interfaces;
using Atron.Application.Mapping;
using Atron.Application.Services;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Repositories;
using Atron.Infrastructure.Repositories.ApplicationRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Models.ApplicationModels;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionContainerAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            // O método AddScoped indica que os serviços são criados uma vez por requisição HTTP
            // O método Singleton indica que o serviço é criado uma vez para todas as requisições
            // O método Transiente indica que sempre será criado um novo serviço cada vez que for necessário

            // Como padrão vou manter o AddScoped pois atende melhor a aplicação com um todo 
            services.AddDbContext<AtronDbContext>(options =>
            // Define o provedor e a string de conexão
            options.UseSqlServer(configuration.GetConnectionString("AtronConnection"),
            // Define o asembly de onde as migrações devem ser mantidas 
            m => m.MigrationsAssembly(typeof(AtronDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<AtronDbContext>()
                    .AddDefaultTokenProviders();

            // Serviços utilitários 
            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));

            // Registra os repositories e services da API
            services = services.AddDependencyInjectionApiDoc();
            services = services.AddServiceMappings();
            services = services.AddMessageValidationServices();
            services = services.AddInfrastructureSecurity(configuration);

            ConfigureTarefaServices(services);
            ConfigureSalarioServices(services);
            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigureUsuarioServices(services);
            ConfigureUsuarioCargoDepartamentoServices(services);
            ConfigureTarefaRepositoryServices(services);
            ConfigureTarefaEstadoServices(services);
            ConfigureSalarioRepositoryServices(services);
            ConfigureMesRepositoryServices(services);
            ConfigurePermissaoServices(services);
            ConfigurePermissaoRepositoryServices(services);
            ConfigurePermissaoEstadoServices(services);

            ConfigureDefaultUserRoleServices(services);
            ConfigureAuthenticationServices(services);
            ConfigureUserAuthenticationServices(services);

            return services;
        }

        private static void ConfigureUsuarioCargoDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioCargoDepartamentoRepository, UsuarioCargoDepartamentoRepository>();
        }

        private static void ConfigureUserAuthenticationServices(IServiceCollection services)
        {
            services.AddScoped<ILoginApplicationRepository, LoginApplicationRepository>();
            services.AddScoped<IRegisterApplicationRepository, RegisterApplicationRepository>();
        }

        private static void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services.AddScoped<ILoginUserService, LoginUserService>();
            services.AddScoped<IRegisterUserService, RegisterUserService>();
        }

        private static void ConfigureDefaultUserRoleServices(IServiceCollection services)
        {
            services.AddScoped<ICreateDefaultUserRoleRepository, CreateDefaultUserRoleRepository>();
        }

        private static void ConfigurePermissaoEstadoServices(IServiceCollection services)
        {
            services.AddScoped<IPermissaoEstadoRepository, PermissaoEstadoRepository>();
        }

        private static void ConfigurePermissaoServices(IServiceCollection services)
        {
            services.AddScoped<IPermissaoRepository, PermissaoRepository>();
            services.AddScoped<IPermissaoService, PermissaoService>();
        }

        private static void ConfigureMesRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<IMesRepository, MesRepository>();
        }

        private static void ConfigureSalarioRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ISalarioRepository, SalarioRepository>();
            services.AddScoped<ISalarioService, SalarioService>();
        }

        private static void ConfigureTarefaEstadoServices(IServiceCollection services)
        {
            services.AddScoped<ITarefaEstadoService, TarefaEstadoService>();
            services.AddScoped<ITarefaEstadoRepository, TarefaEstadoRepository>();

            //// Utilização dos repositories padronizados
            services.AddScoped(typeof(IService<TarefaEstado>), typeof(Service<TarefaEstado>));
            services.AddScoped(typeof(IRepository<TarefaEstado>), typeof(Repository<TarefaEstado>));
        }

        private static void ConfigureTarefaRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<ITarefaService, TarefaService>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        }

        private static void ConfigureCargoServices(IServiceCollection services)
        {
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<ICargoService, CargoService>();
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IDepartamentoService, DepartamentoService>();
        }

        private static void ConfigureSalarioServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Salario>, Repository<Salario>>();
        }

        private static void ConfigureTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();
        }

        private static void ConfigurePermissaoRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Permissao>, Repository<Permissao>>();
        }
    }
}