using Application.DTO;
using Application.Interfaces.Mapping;
using Application.Mapping;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;

namespace IoC
{
    public static class DependencyInjectionMappingContainer
    {
        public static IServiceCollection AddServiceMappings(this IServiceCollection services)
        {
            services.AddScoped<IMapperEngineService, MapperEngine>();
            services.AddScoped<IAsyncApplicationMapService<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            services.AddScoped<IAsyncMap<DepartamentoDTO, Departamento>, DepartamentoMapping>();
            services.AddScoped<IAsyncApplicationMapService<CargoDTO, Cargo>, CargoMapping>();
            services.AddScoped<IAsyncMap<CargoDTO, Cargo>, CargoMapping>();
            services.AddScoped<IAsyncApplicationMapService<PlanejamentoCustoDTO, PlanejamentoCusto>, PlanejamentoCustoMapping>();
            services.AddScoped<IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto>, PlanejamentoCustoMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, Usuario>, UsuarioMapping>();
            services.AddScoped<IAsyncMap<UsuarioDTO, Usuario>, UsuarioMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, UsuarioIdentity>, UsuarioIdentityMapping>();
            services.AddScoped<IAsyncApplicationMapService<TarefaDTO, Tarefa>, TarefaMapping>();
            services.AddScoped<IAsyncApplicationMapService<ModuloDTO, Modulo>, ModuloMapping>();
            services.AddScoped<IAsyncApplicationMapService<UsuarioDTO, PerfilDeAcessoUsuario>, UsuarioDoPerfilDeAcessoMapping>();
            services.AddScoped<PerfilDeAcessoMapping>();
            services.AddScoped<IPerfilDeAcessoMapping>(provider => provider.GetRequiredService<PerfilDeAcessoMapping>());
            services.AddScoped<IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>>(
                provider => provider.GetRequiredService<PerfilDeAcessoMapping>());
            return services;
        }
    }
}
