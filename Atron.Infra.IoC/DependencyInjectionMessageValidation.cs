using Atron.Application.Validations;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
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
            return services;
        }

        private static void ConfigureLoginMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessages, LoginMessageValidation>();
            services.AddScoped<MessageModel, LoginMessageValidation>();
            services.AddScoped<IValidateModel<ApiLogin>, LoginMessageValidation>();
        }

        private static void ConfigureApiRegisterMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessages, ApiRegisterMessageValidation>();
            services.AddScoped<MessageModel, ApiRegisterMessageValidation>();
            services.AddScoped<IValidateModel<ApiRegister>, ApiRegisterMessageValidation>();
        }

        private static void ConfigurarSalarioServices(IServiceCollection services)
        {
            //  services.AddScoped<IPaginationService<SalarioDTO>, PaginationService<SalarioDTO>>();
            services.AddScoped<IMessages, SalarioMessageValidation>();
            services.AddScoped<IValidateModel<Salario>, SalarioMessageValidation>();
            services.AddScoped<MessageModel, SalarioMessageValidation>();
        }

        private static void ConfigurarTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, TarefaMessageValidation>();
            services.AddScoped<MessageModel, TarefaMessageValidation>();
            services.AddScoped<MessageModel, TarefaEstadoMessageValidation>();

            services.AddScoped<IValidateModel<Tarefa>, TarefaMessageValidation>();
            services.AddScoped<IValidateModel<TarefaEstado>, TarefaEstadoMessageValidation>();
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, DepartamentoMessageValidation>();
            services.AddScoped<MessageModel, DepartamentoMessageValidation>();
            services.AddScoped<IValidateModel<Departamento>, DepartamentoMessageValidation>();
        }

        private static void ConfgureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, CargoMessageValidation>();
            services.AddScoped<MessageModel, CargoMessageValidation>();
            services.AddScoped<IValidateModel<Cargo>, CargoMessageValidation>();
        }

        private static void ConfigureUsuarioServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, UsuarioMessageValidation>();
            services.AddScoped<MessageModel, UsuarioMessageValidation>();
            services.AddScoped<IValidateModel<Usuario>, UsuarioMessageValidation>();
        }
    }
}
