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
            services.AddScoped<IAsyncApplicationMapService<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            services.AddScoped<IAsyncApplicationMapService<CargoDTO, Cargo>, CargoMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity>, UsuarioMapping>();
            services.AddScoped<IAsyncApplicationMapService<TarefaDTO, Tarefa>, TarefaMapping>();
            services.AddScoped<IAsyncApplicationMapService<SalarioDTO, Salario>, SalarioMapping>();
            services.AddScoped<IAsyncApplicationMapService<ModuloDTO, Modulo>, ModuloMapping>();
            services.AddScoped<IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>, PerfilDeAcessoMapping>();
            return services;
        }
    }
}