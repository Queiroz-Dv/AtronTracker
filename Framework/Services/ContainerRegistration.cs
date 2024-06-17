using Entities;
using Factory.Entities;
using Factory.Interfaces;
using Helpers.Entity;
using Helpers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Models;
using Notification.Interfaces;
using Notification.Models;
using PersonalTracking.BLL.Interfaces;
using PersonalTracking.BLL.Services;
using PersonalTracking.BLL.Validation;
using PersonalTracking.DAL.Factory;
using PersonalTracking.DAL.Interfaces;
using PersonalTracking.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public static class ContainerRegistration
    {
        public static IServiceCollection AddDesktopServices()
        {
            var services = new ServiceCollection();

            // Registre os serviços desktop aqui
            services.AddScoped<INotificationService, NotificationModel>();
            services.AddScoped<IEntityMessages, InformationMessage>();

            services.AddScoped<IModelFactory<Department, DepartmentDto>, DeparmentFactory>();
            //services.AddScoped<IModelFactory<Position, POSITION>, PositionFactory>();

            services.AddScoped<IValidateHelper<Department>, DepartmentValidationService>();
            //services.AddScoped<IValidateHelper<Position>, PositionValidationService>();

            //services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            //services.AddScoped<IEmployeeService, EmployeeService>();

            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IDepartmentService, DepartmentService>();

            //services.AddScoped<IPositionRepository, PositionRepository>();
            //services.AddScoped<IPositionService, PositionService>();

            //services.AddScoped<Context>();

            return services;
        }
    }
}
