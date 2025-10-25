using Application.DTO;
using Application.DTO.ApiDTO;
using Microsoft.Extensions.DependencyInjection;
using Shared.Interfaces.Services;
using Shared.Services;

namespace IoC
{
    public static class DependencyInjectionPaginationContainer
    {
        public static IServiceCollection AddPaginationServices(this IServiceCollection services)
        {
            CreateDtoPaginationServices(services);
            CreateFilterServices(services);
            return services;
        }

        private static void CreateFilterServices(IServiceCollection services)
        {
            services.AddScoped<IFilterService<SalarioDTO>, FilterService<SalarioDTO>>();
            services.AddScoped<IFilterService<DepartamentoDTO>, FilterService<DepartamentoDTO>>();
            services.AddScoped<IFilterService<CargoDTO>, FilterService<CargoDTO>>();
            services.AddScoped<IFilterService<UsuarioDTO>, FilterService<UsuarioDTO>>();
            services.AddScoped<IFilterService<TarefaDTO>, FilterService<TarefaDTO>>();
            services.AddScoped<IFilterService<LoginDTO>, FilterService<LoginDTO>>();
            services.AddScoped<IFilterService<RegisterDTO>, FilterService<RegisterDTO>>();
            services.AddScoped<IFilterService<ModuloDTO>, FilterService<ModuloDTO>>();
            services.AddScoped<IFilterService<PerfilDeAcessoDTO>, FilterService<PerfilDeAcessoDTO>>();
        }

        private static void CreateDtoPaginationServices(IServiceCollection services)
        {
            services.AddScoped<IPaginationService<DepartamentoDTO>, PaginationService<DepartamentoDTO>>();
            services.AddScoped<IPaginationService<CargoDTO>, PaginationService<CargoDTO>>();
            services.AddScoped<IPaginationService<UsuarioDTO>, PaginationService<UsuarioDTO>>();
            services.AddScoped<IPaginationService<SalarioDTO>, PaginationService<SalarioDTO>>();
            services.AddScoped<IPaginationService<TarefaDTO>, PaginationService<TarefaDTO>>();
            services.AddScoped<IPaginationService<LoginDTO>, PaginationService<LoginDTO>>();
            services.AddScoped<IPaginationService<RegisterDTO>, PaginationService<RegisterDTO>>();
            services.AddScoped<IPaginationService<ModuloDTO>, PaginationService<ModuloDTO>>();
            services.AddScoped<IPaginationService<PerfilDeAcessoDTO>, PaginationService<PerfilDeAcessoDTO>>();
        }
    }
}