using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PersonalTracking.BLL.Interfaces;
using PersonalTracking.BLL.Services;
using PersonalTracking.DAL.DAO;
using PersonalTracking.DAL.DataAcess;
using PersonalTracking.DAL.Interfaces;
using PersonalTracking.DAL.Repositories;
using Unity;

namespace SRV.API
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection ConfigureServicesAPI(this IServiceCollection services, IConfiguration configuration)
        {
            // Registre os serviços aqui
            //services.AddScoped<IEmployeeRepository<EMPLOYEE>, EmployeeRepository>();
            //services.AddScoped<IEmployeeService<EMPLOYEE>, EmployeeService>();
            services.AddScoped<IDepartmentRepository<DEPARTMENT>, DepartmentRepository>();
            //services.AddScoped<IPositionRepository<POSITION>, PositionRepository>();
            services.AddScoped<Context>();

            return services;
        }
    }
}
