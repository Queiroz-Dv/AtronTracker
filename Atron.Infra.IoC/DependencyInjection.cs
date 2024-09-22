using Atron.Application.DTO;
using Atron.Application.Mapping;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Validations;
using Communication.Interfaces;
using Communication.Interfaces.Services;
using Communication.Models;
using Communication.Services;
using ExternalServices.Interfaces;
using ExternalServices.Interfaces.ApiRoutesInterfaces;
using ExternalServices.Services;
using ExternalServices.Services.ApiRouteServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces;
using Shared.Models;
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
            services.AddScoped<IUsuarioExternalService, UsuarioExternalService>();

            services.AddScoped<IApiRouteExternalService, ApiRouteExternalService>();
            services.AddScoped<IPaginationService<ApiRoute>, PaginationService<ApiRoute>>();

            services.AddScoped<IPaginationService<CargoDTO>, PaginationService<CargoDTO>>();
            services.AddScoped<IPaginationService<DepartamentoDTO>, PaginationService<DepartamentoDTO>>();
            services.AddScoped<IPaginationService<UsuarioDTO>, PaginationService<UsuarioDTO>>();

            services.AddScoped<IResultResponseService, ResultResponseModel>();
            services.AddScoped<IUrlModuleFactory, UrlFactory>();
            ConfigureDepartamentoServices(services);
            ConfgureCargoServices(services);

            return services;
        }

        private static void ConfigureDepartamentoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, DepartamentoMessageValidation>();
            services.AddScoped<MessageModel<Departamento>, DepartamentoMessageValidation>();
        }

        private static void ConfgureCargoServices(IServiceCollection services)
        {
            services.AddScoped<IMessages, CargoMessageValidation>();
            services.AddScoped<MessageModel<Cargo>, CargoMessageValidation>();
        }
    }
}