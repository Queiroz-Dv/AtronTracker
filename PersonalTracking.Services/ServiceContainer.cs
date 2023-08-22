using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.DTO;
using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using DAL.Repositories;
using PersonalTracking.Helper.Entity;
using PersonalTracking.Helper.Helpers;
using PersonalTracking.Helper.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PersonalTracking.Models;

namespace PersonalTracking.Services
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddDesktopServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços desktop aqui
            services.AddScoped<IObjectModelHelper<DepartmentModel, DEPARTMENT>, ObjectModelHelper<DepartmentModel, DEPARTMENT>>();
            services.AddScoped<IObjectModelHelper<PositionModel, PositionDTO>, ObjectModelHelper<PositionModel, PositionDTO>>();
            services.AddScoped<IEntityMessages, InformationMessage>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IDepartmentFactory, DepartmentFactory>();

            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IPositionFactory, PositionFactory>();

            services.AddScoped<Context>();

            return services;
        }
    }
}
