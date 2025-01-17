﻿using Atron.Application.ApiInterfaces;
using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices;
using Atron.Application.ApiServices.ApplicationServices;
using Atron.Application.Interfaces;
using Atron.Application.Mapping;
using Atron.Application.Services;
using Atron.Application.Validations;
using Atron.Domain.ApiEntities;
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
using Shared.Interfaces;
using Shared.Models;
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

            // TODO: Criar a controller da Home no projeto de API
            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Home/Index");

            //Repositórios e serviços padrões
            services.AddScoped<IRepository<Permissao>, Repository<Permissao>>();
            services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();
            services.AddScoped<IRepository<Salario>, Repository<Salario>>();

            // Registra os repositories e services
            services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();
            services.AddScoped<IDepartamentoService, DepartamentoService>();

            services.AddScoped<ICargoRepository, CargoRepository>();
            services.AddScoped<ICargoService, CargoService>();

            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();

            services.AddScoped<ITarefaRepository, TarefaRepository>();
            services.AddScoped<ITarefaService, TarefaService>();

            services.AddScoped<ITarefaEstadoService, TarefaEstadoService>();
            services.AddScoped<ITarefaEstadoRepository, TarefaEstadoRepository>();

            //// Utilização dos repositories padronizados
            services.AddScoped(typeof(IService<TarefaEstado>), typeof(Service<TarefaEstado>));
            services.AddScoped(typeof(IRepository<TarefaEstado>), typeof(Repository<TarefaEstado>));

            services.AddScoped<ISalarioRepository, SalarioRepository>();
            services.AddScoped<ISalarioService, SalarioService>();

            services.AddScoped<IMesRepository, MesRepository>();

            services.AddScoped<IPermissaoRepository, PermissaoRepository>();
            services.AddScoped<IPermissaoService, PermissaoService>();

            services.AddScoped<IPermissaoEstadoRepository, PermissaoEstadoRepository>();

            

            // Serviços utilitários 
            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));

            ConfigureDepartamentoServices(services);
            ConfigureCargoServices(services);
            ConfigureUsuarioServices(services);
            ConfigurarTarefaServices(services);
            ConfigurarSalarioServices(services);

            services.AddScoped<IMessages, ApiRegisterMessageValidation>();
            services.AddScoped<MessageModel<ApiRegister>, ApiRegisterMessageValidation>();

            services.AddScoped<IMessages, LoginMessageValidation>();
            services.AddScoped<MessageModel<ApiLogin>, LoginMessageValidation>();

            services.AddScoped<ICreateDefaultUserRoleRepository, CreateDefaultUserRoleRepository>();
            services.AddScoped<ILoginUserService, LoginUserService>();
            services.AddScoped<IRegisterUserService, RegisterUserService>();
            services.AddScoped<ILoginApplicationRepository, LoginApplicationRepository>();
            services.AddScoped<IRegisterApplicationRepository, RegisterApplicationRepository>();
            services.AddScoped<IUsuarioCargoDepartamentoRepository, UsuarioCargoDepartamentoRepository>();

            return services;
        }

        private static void ConfigurarSalarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, SalarioMessageValidation>();
            services.AddScoped<MessageModel<Salario>, SalarioMessageValidation>();
        }

        private static void ConfigurarTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, TarefaMessageValidation>();
            services.AddScoped<MessageModel<Tarefa>, TarefaMessageValidation>();
            services.AddScoped<MessageModel<TarefaEstado>, TarefaEstadoMessageValidation>();
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, DepartamentoMessageValidation>();
            services.AddScoped<MessageModel<Departamento>, DepartamentoMessageValidation>();
        }
        private static void ConfigureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, CargoMessageValidation>();
            services.AddScoped<MessageModel<Cargo>, CargoMessageValidation>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, UsuarioMessageValidation>();
            services.AddScoped<MessageModel<Usuario>, UsuarioMessageValidation>();
        }
    }
}