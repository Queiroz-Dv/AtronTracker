﻿using Atron.Application.DTO;
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
            services.AddScoped<IApplicationMapService<UsuarioDTO, Usuario>, UsuarioMapping>();
            services.AddScoped<IApplicationMapService<TarefaDTO, Tarefa>, TarefaMapping>();
            services.AddScoped<IApplicationMapService<SalarioDTO, Salario>, SalarioMapping>();
            return services;
        }
    }
}