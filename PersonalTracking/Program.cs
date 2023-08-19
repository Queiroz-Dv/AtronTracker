using BLL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using PersonalTracking.Services;
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
            var services = ServiceContainer.AddDesktopServices();

            // Constroí o ServiceProvider partindo do serviceCollection
            var serviceProvider = services.BuildServiceProvider();

            // Configura a aplicação Win Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Obtém uma instância do serviço necessário usando a injeção de dependência
            var employeeService = serviceProvider.GetService<IEmployeeService>();
            var departmentService = serviceProvider.GetService<IDepartmentService>();
            var positionService = serviceProvider.GetService<IPositionService>();

            ////Executa o form de login passando o serviço
            Application.Run(new FrmLogin(employeeService, departmentService, positionService));

            var frase = "Eu amo você também Naylane!";
            Digite(frase);
        }

        internal static void Digite(string TeAmo)
        {
            var fraseComData = TeAmo + DateTime.Now;
        }
    }
}
