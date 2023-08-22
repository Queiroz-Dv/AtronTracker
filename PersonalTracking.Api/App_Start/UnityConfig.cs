using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.Factory;
using DAL.Interfaces;
using DAL.Interfaces.FactoryModules;
using DAL.Repositories;
using PersonalTracking.Helper.Helpers;
using PersonalTracking.Helper.Interfaces;
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
            container.RegisterType<IDepartmentFactory, DepartmentFactory>();
            container.RegisterType<IObjectModelHelper<DepartmentModel, DEPARTMENT>, ObjectModelHelper<DepartmentModel, DEPARTMENT>>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}