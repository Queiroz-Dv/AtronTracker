using Atron.Application.Mapping;
using Atron.Application.ViewInterfaces;
using Atron.Application.ViewServices;
using Atron.Domain.ViewsInterfaces;
using Atron.Infrastructure.ViewsRepositories;
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
            
            services.AddAutoMapper(typeof(DomainToDtoMappingProfile));
            services.AddScoped<IDepartamentoViewService, DepartamentoViewService>();
            services.AddScoped<IDepartamentoViewRepository, DepartamentoViewRepository>();           
            return services;
        }
    }
}