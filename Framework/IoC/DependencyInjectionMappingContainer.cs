using Application.DTO;
using Application.Mapping;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Mapper;
using Shared.Services.Mapper;

namespace IoC
{
    public static class DependencyInjectionMappingContainer
    {
        public static IServiceCollection AddServiceMappings(this IServiceCollection services)
        {
            services.AddScoped<IMapperEngine, MapperEngine>();
            services.AddScoped<IAsyncApplicationMapService<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            services.AddScoped<IAsyncApplicationMapService<CargoDTO, Cargo>, CargoMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, Usuario>, UsuarioMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity>, UsuarioIdentityMapping>();
            services.AddScoped<IAsyncApplicationMapService<TarefaDTO, Tarefa>, TarefaMapping>();
            services.AddScoped<IAsyncApplicationMapService<SalarioDTO, Salario>, SalarioMapping>();
            services.AddScoped<IAsyncApplicationMapService<ModuloDTO, Modulo>, ModuloMapping>();
            services.AddScoped<IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>, PerfilDeAcessoMapping>();
            return services;
        }
    }
}