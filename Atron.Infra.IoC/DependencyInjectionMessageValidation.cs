using Atron.Application.Validations;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.DTO.API;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Infra.IoC
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
            services.AddScoped<IMessages, LoginMessageValidation>();
            services.AddScoped<IValidateModel<ApiLogin>, LoginMessageValidation>();
            services.AddScoped<MessageModel, LoginMessageValidation>();

            services.AddScoped<IMessages, InfoTokenMessageValidation>();
            services.AddScoped<IValidateModel<DadosDoTokenDTO>, InfoTokenMessageValidation>();
            services.AddScoped<MessageModel, InfoTokenMessageValidation>();
        }

        private static void ConfigureApiRegisterMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessages, ApiRegisterMessageValidation>();
            services.AddScoped<IValidateModel<ApiRegister>, ApiRegisterMessageValidation>();
            services.AddScoped<MessageModel, ApiRegisterMessageValidation>();
        }

        private static void ConfigurarSalarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, SalarioMessageValidation>();
            services.AddScoped<IValidateModel<Salario>, SalarioMessageValidation>();
            services.AddScoped<MessageModel, SalarioMessageValidation>();
        }

        private static void ConfigurarTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, TarefaMessageValidation>();
            services.AddScoped<IValidateModel<Tarefa>, TarefaMessageValidation>();
            services.AddScoped<MessageModel, TarefaMessageValidation>();

        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, DepartamentoMessageValidation>();
            services.AddScoped<IValidateModel<Departamento>, DepartamentoMessageValidation>();
            services.AddScoped<MessageModel, DepartamentoMessageValidation>();
        }

        private static void ConfgureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, CargoMessageValidation>();
            services.AddScoped<IValidateModel<Cargo>, CargoMessageValidation>();
            services.AddScoped<MessageModel, CargoMessageValidation>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, UsuarioMessageValidation>();
            services.AddScoped<IValidateModel<Usuario>, UsuarioMessageValidation>();
            services.AddScoped<MessageModel, UsuarioMessageValidation>();
        }

        private static void ConfigureModuloServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, ModuloMessageValidation>();
            services.AddScoped<IValidateModel<Modulo>, ModuloMessageValidation>();
            services.AddScoped<MessageModel, ModuloMessageValidation>();
        }

        private static void ConfigurePerfilDeAcessoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, PerfilDeAcessoMessageValidation>();
            services.AddScoped<IValidateModel<PerfilDeAcesso>, PerfilDeAcessoMessageValidation>();
            services.AddScoped<MessageModel, PerfilDeAcessoMessageValidation>();
        }
    }
}