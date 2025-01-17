using Atron.Application.DTO;
using Atron.Application.Mapping;
using Atron.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionMappingContainer
    {
        public static IServiceCollection AddServiceMappings(this IServiceCollection services)
        {
            //IApplicationMapService<DepartamentoDTO, Departamento>,
            services.AddScoped<IApplicationMapService<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            return services;
        }
    }
}
