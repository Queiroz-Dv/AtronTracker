using Atron.Application.Mapping;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using Communication.Models;
using Communication.Services;
using ExternalServices.Interfaces;
using ExternalServices.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Services;

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

            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));

            services.AddScoped<IApiClient, ApiClient>();
            services.AddScoped<ICommunicationService, CommunicationService>();
            
            services.AddScoped<IDepartamentoExternalService, DepartamentoExternalService>();
            services.AddScoped<ICargoExternalService, CargoExternalService>();

            // services.AddScoped<PaginationService>();
            return services;
        }
    }
}