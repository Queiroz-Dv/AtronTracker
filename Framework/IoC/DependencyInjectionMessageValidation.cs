using Application.Validations;
using Domain.ApiEntities;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.DTO.API;
using Shared.Models;

namespace IoC
{
    public static class DependencyInjectionMessageValidation
    {
        public static IServiceCollection AddMessageValidationServices(this IServiceCollection services)
        {
            ConfigureLoginMessageValidation(services);
            ConfigureApiRegisterMessageValidation(services);
            ConfigureDepartamentoServices(services);
            ConfgureCargoServices(services);
            ConfigureUsuarioServices(services);
            ConfigurarTarefaServices(services);
            ConfigurarSalarioServices(services);
            ConfigureModuloServices(services);
            ConfigurePerfilDeAcessoServices(services);
            return services;
        }

        private static void ConfigureLoginMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, LoginMessageValidation>();
            services.AddScoped<IValidateModelService<ApiLogin>, LoginMessageValidation>();
            services.AddScoped<MessageModel, LoginMessageValidation>();

            services.AddScoped<IMessageBaseService, InfoTokenMessageValidation>();
            services.AddScoped<IValidateModelService<DadosDoTokenDTO>, InfoTokenMessageValidation>();
            services.AddScoped<MessageModel, InfoTokenMessageValidation>();
        }

        private static void ConfigureApiRegisterMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, ApiRegisterMessageValidation>();
            services.AddScoped<IValidateModelService<UsuarioRegistro>, ApiRegisterMessageValidation>();
            services.AddScoped<MessageModel, ApiRegisterMessageValidation>();
        }

        private static void ConfigurarSalarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, SalarioMessageValidation>();
            services.AddScoped<IValidateModelService<Salario>, SalarioMessageValidation>();
            services.AddScoped<MessageModel, SalarioMessageValidation>();
        }

        private static void ConfigurarTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, TarefaMessageValidation>();
            services.AddScoped<IValidateModelService<Tarefa>, TarefaMessageValidation>();
            services.AddScoped<MessageModel, TarefaMessageValidation>();

        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, DepartamentoMessageValidation>();
            services.AddScoped<IValidateModelService<Departamento>, DepartamentoMessageValidation>();
            services.AddScoped<MessageModel, DepartamentoMessageValidation>();
        }

        private static void ConfgureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, CargoMessageValidation>();
            services.AddScoped<IValidateModelService<Cargo>, CargoMessageValidation>();
            services.AddScoped<MessageModel, CargoMessageValidation>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, UsuarioMessageValidation>();
            services.AddScoped<IValidateModelService<Usuario>, UsuarioMessageValidation>();
            services.AddScoped<MessageModel, UsuarioMessageValidation>();
        }

        private static void ConfigureModuloServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, ModuloMessageValidation>();
            services.AddScoped<IValidateModelService<Modulo>, ModuloMessageValidation>();
            services.AddScoped<MessageModel, ModuloMessageValidation>();
        }

        private static void ConfigurePerfilDeAcessoServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, PerfilDeAcessoMessageValidation>();
            services.AddScoped<IValidateModelService<PerfilDeAcesso>, PerfilDeAcessoMessageValidation>();
            services.AddScoped<MessageModel, PerfilDeAcessoMessageValidation>();
        }
    }
}