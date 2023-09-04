using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Repositories;
using PersonalTracking.Entities;
using PersonalTracking.Factory.Entities;
using PersonalTracking.Factory.Interfaces;
using PersonalTracking.Models;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace PersonalTracking.Api
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IDepartmentService, DepartmentService>();
            container.RegisterType<IDepartmentRepository, DepartmentRepository>();
            container.RegisterType<IModelFactory<DepartmentModel, DEPARTMENT>, DeparmentFactory>();
            
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}