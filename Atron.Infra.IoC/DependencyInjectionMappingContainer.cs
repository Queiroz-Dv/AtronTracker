using Atron.Application.DTO;
using Atron.Application.Mapping;
using Atron.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Mapper;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionMappingContainer
    {
        public static IServiceCollection AddServiceMappings(this IServiceCollection services)
        {
            services.AddScoped<IApplicationMapService<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            services.AddScoped<IApplicationMapService<CargoDTO, Cargo>, CargoMapping>();
            services.AddScoped<IApplicationMapService<UsuarioDTO, UsuarioIdentity>, UsuarioMapping>();
            services.AddScoped<IApplicationMapService<TarefaDTO, Tarefa>, TarefaMapping>();
            services.AddScoped<IApplicationMapService<SalarioDTO, Salario>, SalarioMapping>();
            services.AddScoped<IApplicationMapService<ModuloDTO, Modulo>, ModuloMapping>();
            services.AddScoped<IApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>, PerfilDeAcessoMapping>();    
            return services;
        }
    }
}