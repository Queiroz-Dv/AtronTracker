using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Application.Services;
using Application.Services.AuthServices;
using Application.Services.EntitiesServices;
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
using Microsoft.Extensions.DependencyInjection;
using Shared.Models.ApplicationModels;
using System;
using System.IO;
using System.Text.Json.Serialization;

namespace IoC
{
    public static class DependencyInjectionContainerAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services, IConfiguration configuration)
        {
            string sqlConnection = configuration.GetConnectionString("AtronConnection");

            services.AddDbContext<AtronDbContext>(options =>
                options.UseSqlServer(sqlConnection,
                b => b.MigrationsAssembly(typeof(AtronDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<AtronDbContext>()
                    .AddDefaultTokenProviders();

            services = services.AddSharedInfrastructure(configuration);

            // Evitar o looping infinito 
            services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddScoped(provider => provider.GetRequiredService<IHttpContextAccessor>().HttpContext?.Response.Cookies);

            // Registra os repositories e services da API
            services = services.AddDependencyInjectionApiDoc();
            services = services.AddServiceMappings();
            services = services.AddMessageValidationServices();
            services = services.AddInfrastructureSecurity(configuration);
            ConfigureModuloServices(services);
            ConfigureTarefaServices(services);
            ConfigureSalarioServices(services);
            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigureUsuarioServices(services);
            ConfigureUsuarioCargoDepartamentoServices(services);
            ConfigureTarefaRepositoryServices(services);
            ConfigureSalarioRepositoryServices(services);
            ConfigureDefaultUserRoleServices(services);
            ConfigureAuthenticationServices(services);
            ConfigurePerfilDeAcessoServices(services);
            ConfigurePerfilDeAcessoUsuarioServices(services);

            services = services.AddContexts();

            // Registra os serviços essenciais do sistema de proteção de dados (Data Protection) na injeção de dependência.
            services.AddDataProtection()

                // Define um nome de aplicativo exclusivo ("Atron").
                // Usado para isolar os cookies e tokens da sua aplicação de outras aplicações rodando no mesmo servidor.
                .SetApplicationName("Atron")

                // Instruiu o sistema a salvar (persistir) as chaves de criptografia em uma pasta local chamada "./keys".
                // Isso é vital para produção, garantindo que os usuários não sejam deslogados a cada reinício da aplicação.
                .PersistKeysToFileSystem(new DirectoryInfo(@"./keys"))

                // Configura o "rodízio de chaves" (key rotation), gerando uma nova chave de criptografia a cada 90 dias.
                // É uma boa prática de segurança para limitar o tempo de vida de qualquer chave.
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
        }

        private static void ConfigureDefaultUserRoleServices(IServiceCollection services)
        {
            services.AddScoped<ICreateDefaultUserRoleRepository, CreateDefaultUserRoleRepository>();
        }

        private static void ConfigureSalarioRepositoryServices(IServiceCollection services)
        {
            services.AddScoped<ISalarioRepository, SalarioRepository>();
            services.AddScoped<ISalarioService, SalarioService>();
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
            services.AddScoped(typeof(IRepository<Usuario>), typeof(Repository<Usuario>));
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

        private static void ConfigureSalarioServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Salario>, Repository<Salario>>();
        }

        private static void ConfigureTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();
        }
    }
}