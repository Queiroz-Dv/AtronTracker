using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using DAL.Repositories;
using HLP.Entity;
using HLP.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace PersonalTracking.Services
{
    public class ServiceContainer
    {
        public static IServiceCollection AddDesktopServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços desktop aqui
            services.AddScoped<IEmployeeRepository<EMPLOYEE>, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEntityMessages, InformationMessage>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IDepartmentFactory, DepartmentFactory>();
            services.AddScoped<Context>();

            return services;
        }
    }
}
