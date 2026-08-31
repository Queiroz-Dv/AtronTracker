using Application.DTO;
using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interfaces.Mapping;
using Application.Mapping;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Interfaces.Mapping;

namespace Infrastructure.DependencyInjection
{
    internal static class TrackerMappingServiceCollectionExtensions
    {
        public static IServiceCollection AddTrackerMappings(this IServiceCollection services)
        {
            services.AddMapper<Departamento, DepartamentoDTO, DepartamentoMapping>();
            services.AddMapper<Empresa, EmpresaDTO, EmpresaMapping>();
            services.AddMapper<Cargo, CargoDTO, CargoMapping>();
            services.AddMapper<Modulo, ModuloDTO, ModuloMapping>();
            services.AddMapper<PerfilDeAcessoUsuario, UsuarioDTO, UsuarioDoPerfilDeAcessoMapping>();
            services.AddMapper<PerfilDeAcesso, PerfilDeAcessoDTO, PerfilDeAcessoMapping>();
            services.AddMapper<Usuario, UsuarioDTO, UsuarioMapping>();
            services.AddMapper<Usuario, UsuarioRequest, UsuarioRequestMapping>();
            services.AddMapper<UsuarioIdentity, UsuarioDTO, UsuarioIdentityMapping>();
            services.AddMapper<TarefaEstado, TarefaEstadoDTO, TarefaEstadoMapping>();
            services.AddMapper<Tarefa, TarefaDTO, TarefaMapping>();
            services.AddMapper<PlanejamentoCusto, PlanejamentoCustoDTO, PlanejamentoCustoMapping>();
            services.AddScoped<WorkspaceMapping>();
            services.AddScoped<IToEntityMapper<Workspace, CriarWorkspaceInicialRequest>>(provider =>
                provider.GetRequiredService<WorkspaceMapping>());
            services.AddScoped<IToDtoMapper<Workspace, WorkspaceInicialResponse>>(provider =>
                provider.GetRequiredService<WorkspaceMapping>());
            services.AddScoped<ConviteWorkspaceMapping>();
            services.AddScoped<IToEntityMapper<ConviteWorkspace, CriarConviteWorkspaceRequest>>(provider =>
                provider.GetRequiredService<ConviteWorkspaceMapping>());
            services.AddScoped<IToDtoMapper<ConviteWorkspace, ConviteWorkspaceResponse>>(provider =>
                provider.GetRequiredService<ConviteWorkspaceMapping>());

            services.AddScoped<IPerfilDeAcessoMapping>(provider =>
                provider.GetRequiredService<PerfilDeAcessoMapping>());

            services.AddScoped<SolicitacaoObtencaoTarefaMapping>();
            services.AddScoped<IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO>>(provider =>
                provider.GetRequiredService<SolicitacaoObtencaoTarefaMapping>());

            services.AddScoped<TarefaMovimentacaoMapping>();
            services.AddScoped<ITarefaMovimentacaoMapping>(
                provider => provider.GetRequiredService<TarefaMovimentacaoMapping>());
            services.AddScoped<IMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>>(
                provider => provider.GetRequiredService<TarefaMovimentacaoMapping>());
            services.AddScoped<IToDtoMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>>(
                provider => provider.GetRequiredService<TarefaMovimentacaoMapping>());
            services.AddScoped<IToEntityMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>>(
                provider => provider.GetRequiredService<TarefaMovimentacaoMapping>());

            return services;
        }

        private static void AddMapper<TEntity, TDto, TMapper>(this IServiceCollection services)
            where TEntity : class
            where TDto : class
            where TMapper : class, IMapper<TEntity, TDto>
        {
            services.AddScoped<TMapper>();
            services.AddScoped<IMapper<TEntity, TDto>>(provider =>
                provider.GetRequiredService<TMapper>());
            services.AddScoped<IToDtoMapper<TEntity, TDto>>(provider =>
                provider.GetRequiredService<TMapper>());
            services.AddScoped<IToEntityMapper<TEntity, TDto>>(provider =>
                provider.GetRequiredService<TMapper>());
        }
    }
}
