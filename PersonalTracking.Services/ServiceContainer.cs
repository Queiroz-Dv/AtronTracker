using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace PersonalTracking.Services
{
    public class ServiceContainer
    {
        public static IServiceCollection AddServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços aqui
            services.AddScoped<IEmployeeRepository<EMPLOYEE>, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IPositionRepository<POSITION>, PositionRepository>();
            services.AddScoped<Context>();

            return services;
        }

        public static void RegisterDependencies()
        {
            var container = new UnityContainer();
            // 
            container.RegisterType<IDepartmentService, DepartmentService>();
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}
