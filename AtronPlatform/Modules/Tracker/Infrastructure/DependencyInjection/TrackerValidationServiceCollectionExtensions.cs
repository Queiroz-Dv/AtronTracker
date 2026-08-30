using Application.DTO;
using Application.Validador;
using Application.Validations;
using Domain.ApiEntities;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace Infrastructure.DependencyInjection
{
    internal static class TrackerValidationServiceCollectionExtensions
    {
        public static IServiceCollection AddTrackerValidations(this IServiceCollection services)
        {
            ConfigureLoginMessageValidation(services);            
            ConfigureDepartamentoServices(services);
            ConfigureEmpresaServices(services);
            ConfgureCargoServices(services);
            ConfigurePlanejamentoCustoServices(services);
            ConfigurarTarefaServices(services);
            ConfigureModuloServices(services);
            ConfigurePerfilDeAcessoServices(services);
            return services;
        }

        private static void ConfigureLoginMessageValidation(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, LoginMessageValidation>();
            services.AddScoped<IValidateModelService<ApiLogin>, LoginMessageValidation>();
            services.AddScoped<Notifiable, LoginMessageValidation>();

            services.AddScoped<IMessageBaseService, InfoTokenMessageValidation>();
            services.AddScoped<IValidateModelService<DadosDoTokenDTO>, InfoTokenMessageValidation>();
            services.AddScoped<Notifiable, InfoTokenMessageValidation>();
        }

        private static void ConfigurarTarefaServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, TarefaMessageValidation>();
            services.AddScoped<IValidateModelService<Tarefa>, TarefaMessageValidation>();
            services.AddScoped<IValidador<TarefaDTO>, TarefaValidador>();
            services.AddScoped<Notifiable, TarefaMessageValidation>();

        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {            
            services.AddScoped<IValidador<DepartamentoDTO>, DepartamentoValidador>();
        }

        private static void ConfigureEmpresaServices(IServiceCollection services)
        {
            services.AddScoped<IValidador<EmpresaDTO>, EmpresaValidador>();
        }

        private static void ConfgureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IValidador<CargoDTO>, CargoValidador>();
        }

        private static void ConfigurePlanejamentoCustoServices(IServiceCollection services)
        {
            services.AddScoped<IValidador<PlanejamentoCustoDTO>, PlanejamentoCustoValidador>();
        }

        private static void ConfigureModuloServices(IServiceCollection services)
        {
            services.AddScoped<IMessageBaseService, ModuloMessageValidation>();
            services.AddScoped<IValidateModelService<Modulo>, ModuloMessageValidation>();
            services.AddScoped<Notifiable, ModuloMessageValidation>();
        }

        private static void ConfigurePerfilDeAcessoServices(IServiceCollection services)
        {
            services.AddScoped<IValidador<PerfilDeAcessoDTO>, PerfilDeAcessoValidador>();
            services.AddScoped<PerfilDeAcessoMessageValidation>();
            services.AddScoped<IMessageBaseService>(provider => provider.GetRequiredService<PerfilDeAcessoMessageValidation>());
            services.AddScoped<IValidateModelService<PerfilDeAcesso>>(provider => provider.GetRequiredService<PerfilDeAcessoMessageValidation>());
            services.AddScoped<Notifiable>(provider => provider.GetRequiredService<PerfilDeAcessoMessageValidation>());
        }
    }
}
