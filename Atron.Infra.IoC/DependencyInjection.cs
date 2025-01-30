using Atron.Application.DTO;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.Entities;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using Communication.Models;
using Communication.Services;
using ExternalServices.Interfaces;
using ExternalServices.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atron.Infra.IoC
{
    /// <summary>
    /// Classe responsável pela injeção de dependência
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // O método AddScoped indica que os serviços são criados uma vez por requisição HTTP
            // O método Singleton indica que o serviço é criado uma vez para todas as requisições
            // O método Transiente indica que sempre será criado um novo serviço cada vez que for necessário
            // Como padrão vou manter o AddScoped pois atende melhor a aplicação com um todo
            // 
            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<IUrlTransferService, ApiClient>();
            services.AddScoped<IRouterBuilderService, RouterBuilder>();

            services.AddScoped<ILoginExternalService, LoginExternalService>();
            services.AddScoped<IRegisterExternalService, RegisterExternalService>();

            // Configuração dos serviços genéricos 
            services.AddScoped<IExternalService<RegisterDTO>, ExternalService<RegisterDTO>>();
            services.AddScoped<IExternalService<DepartamentoDTO>, ExternalService<DepartamentoDTO>>();
            services.AddScoped<IExternalService<CargoDTO>, ExternalService<CargoDTO>>();
            services.AddScoped<IExternalService<UsuarioDTO>, ExternalService<UsuarioDTO>>();
            services.AddScoped<IExternalService<TarefaDTO>, ExternalService<TarefaDTO>>();
            services.AddScoped<IExternalService<TarefaEstado>, ExternalService<TarefaEstado>>();
            services.AddScoped<IExternalService<SalarioDTO>, ExternalService<SalarioDTO>>();

            services = services.AddMessageValidationServices();
            services = services.AddCustomCookieConfiguration();
            services = services.AddPaginationServices();
            services = services.AddInfrastructureSecurity(configuration);

            return services;
        }
    }
}