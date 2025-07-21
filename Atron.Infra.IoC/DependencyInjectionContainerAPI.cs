﻿using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices.AuthServices;
using Atron.Application.Interfaces.Services;
using Atron.Application.Services.EntitiesServices;
using Atron.Domain.Componentes;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Atron.Infrastructure.Context;
using Atron.Infrastructure.Interfaces;
using Atron.Infrastructure.Repositories;
using Atron.Infrastructure.Repositories.ApplicationRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;

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
            //  services.AddDbContext<AtronDbContext>(options =>
            // Define o provedor e a string de conexão
            //    options.UseSqlServer(configuration.GetConnectionString("AtronConnection"),
            // Define o asembly de onde as migrações devem ser mantidas 
            //   m => m.MigrationsAssembly(typeof(AtronDbContext).Assembly.FullName)));

            //services.AddIdentity<ApplicationUser, ApplicationRole>()
            //        .AddEntityFrameworkStores<AtronDbContext>()
            //        .AddDefaultTokenProviders();

            // Evitar o looping infinito 
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            // With this line:  
            services.AddScoped(provider => provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Response.Cookies);


            // Registra os repositories e services da API
            services = services.AddServiceMappings();
            services = services.AddMessageValidationServices();
            services = services.AddInfrastructureSecurity(configuration);
            ConfigureModuloServices(services);
            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigureUsuarioServices(services);
            ConfigureUsuarioCargoDepartamentoServices(services);
            ConfigureTarefaRepositoryServices(services);
            ConfigureSalarioRepositoryServices(services);
            ConfigureAuthenticationServices(services);
            ConfigurePropriedadesDeFluxoServices(services);
            ConfigurePropriedadesDeFluxoModuloServices(services);
            ConfigurePerfilDeAcessoServices(services);
            ConfigurePerfilDeAcessoUsuarioServices(services);

            services = services.AddContexts();

            services.AddSingleton<LiteDbConnectionStringProvider>();

            services.AddScoped<ILiteDbContext, AtronLiteDbContext>();
            services.AddScoped<ILiteUnitOfWork>(provider =>
            {
                var ctx = provider.GetRequiredService<ILiteDbContext>();
                return new LiteUnitOfWork(((AtronLiteDbContext)ctx)._db);
            });

            services.AddScoped<LiteDataSetContext, AtronLiteDbContext>();
            services.AddScoped<ILiteFacade, LiteFacade>();
            return services;
        }

        private static void ConfigurePerfilDeAcessoUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IPerfilDeAcessoUsuarioRepository, PerfilDeAcessoUsuarioRepository>();
            services.AddScoped<IRepository<PerfilDeAcessoUsuario>, Repository<PerfilDeAcessoUsuario>>();
        }

        private static void ConfigurePropriedadesDeFluxoModuloServices(IServiceCollection services)
        {
            services.AddScoped<IPropriedadeDeFluxoModuloRepository, PropriedadeDeFluxoModuloRepository>();
            services.AddScoped<IRepository<PropriedadeDeFluxoModulo>, Repository<PropriedadeDeFluxoModulo>>();
        }

        private static void ConfigurePropriedadesDeFluxoServices(IServiceCollection services)
        {
            services.AddScoped<IPropriedadeDeFluxoRepository, PropriedadeDeFluxoRepository>();
            services.AddScoped<IRepository<PropriedadesDeFluxo>, Repository<PropriedadesDeFluxo>>();
        }

        private static void ConfigureUsuarioCargoDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IUsuarioCargoDepartamentoRepository, UsuarioCargoDepartamentoRepository>();
            services.AddScoped<IRepository<UsuarioCargoDepartamento>, Repository<UsuarioCargoDepartamento>>();
        }

        private static void ConfigureAuthenticationServices(IServiceCollection services)
        {
            services.AddScoped<ILoginService, LoginService>();
            services.AddScoped<ILoginRepository, LoginRepository>();
            services.AddScoped<IRegistroUsuarioService, RegistroUsuarioService>();
        }

        private static void ConfigureSalarioRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Salario>, Repository<Salario>>();
            services.AddScoped<ISalarioRepository, SalarioRepository>();
            services.AddScoped<ISalarioService, SalarioService>();
        }

        private static void ConfigureTarefaRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped(typeof(IRepository<Tarefa>), typeof(Repository<Tarefa>));
            services.AddScoped<ITarefaService, TarefaService>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Usuario>, Repository<Usuario>>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IUsuarioService, UsuarioService>();
        }

        private static void ConfigureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Cargo>, Repository<Cargo>>();
            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<ICargoService, CargoService>();
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Departamento>, Repository<Departamento>>();
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IDepartamentoService, DepartamentoService>();
        }

        private static void ConfigureModuloServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Modulo>, Repository<Modulo>>();
            services.AddScoped<IModuloRepository, ModuloRepository>();
            services.AddScoped<IModuloService, ModuloService>();
        }

        private static void ConfigurePerfilDeAcessoServices(IServiceCollection services)
        {
            services.AddScoped<IPerfilDeAcessoRepository, PerfilDeAcessoRepository>();
            services.AddScoped<IRepository<PerfilDeAcesso>, Repository<PerfilDeAcesso>>();
            services.AddScoped<IPerfilDeAcessoService, PerfilDeAcessoService>();
        }
    }
}