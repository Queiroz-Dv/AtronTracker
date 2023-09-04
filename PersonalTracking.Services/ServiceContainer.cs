using BLL.Interfaces;
using BLL.Services;
using BLL.Validation;
using DAL.DTO;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using PersonalTracking.Entities;
using PersonalTracking.Factory.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Helper.Entity;
using PersonalTracking.Helper.Interfaces;
using PersonalTracking.Models;
using PersonalTracking.Notification.Interfaces;
using PersonalTracking.Notification.Models;

namespace PersonalTracking.Services
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddDesktopServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços desktop aqui
            services.AddScoped<INotificationService, NotificationModel>();
            services.AddScoped<IEntityMessages, InformationMessage>();

            services.AddScoped<IModelFactory<DepartmentModel, DEPARTMENT>, DeparmentFactory>();
            services.AddScoped<IModelFactory<PositionModel, POSITION>, PositionFactory>();

            services.AddScoped<IValidateHelper<DepartmentModel>, DepartmentValidationService>();
            services.AddScoped<IValidateHelper<PositionModel>, PositionValidationService>();

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IPositionService, PositionService>();

            services.AddScoped<Context>();

            return services;
        }
    }
}
