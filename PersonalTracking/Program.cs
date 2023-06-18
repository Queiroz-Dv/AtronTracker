using BLL.Interfaces;
using BLL.Services;
using DAL;
using DAL.DAO;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows.Forms;

namespace PersonalTracking
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = new ServiceCollection();
            services.AddScoped<IEmployeeRepository<EMPLOYEE>, EmployeeRepository>();
            services.AddScoped<IEmployeeService<EMPLOYEE>, EmployeeService>();
            services.AddScoped<IDepartmentRepository<DEPARTMENT>, DepartmentRepository>();
            services.AddScoped<IPositionRepository<POSITION>, PositionRepository>();
            services.AddScoped<Context>();

            var serviceProvider = services.BuildServiceProvider();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var loginService = serviceProvider.GetService<IEmployeeService<EMPLOYEE>>();

            Application.Run(new FrmLogin(loginService));
            //Application.Run(new FrmRegister());
        }
    }
}
