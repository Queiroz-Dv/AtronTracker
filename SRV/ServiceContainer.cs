using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace SRV
{
    public class ServiceContainer
    {
        public static IServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços aqui
            services.AddScoped<IEmployeeRepository<EMPLOYEE>, EmployeeRepository>();
            services.AddScoped<IEmployeeService<EMPLOYEE>, EmployeeService>();
            services.AddScoped<IDepartmentRepository<DEPARTMENT>, DepartmentRepository>();
            services.AddScoped<IPositionRepository<POSITION>, PositionRepository>();
            services.AddScoped<Context>();

            return services;
        }
    }
}