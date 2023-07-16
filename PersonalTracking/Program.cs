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
            // Cria a instância do ServiceCollection a partir do projeto de serviços
            //var services = ServiceContainer.AddDesktopServices();

            // Configura a aplicação Win Forms
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Obtém uma instância do serviço necessário usando a injeção de dependência
            //var employeeService = services.GetService<IEmployeeService>();
            //var departmentService = services.GetService<IDepartmentService>();

            ////Executa o form de login passando o serviço
            //Application.Run(new FrmLogin(employeeService, departmentService));
        }
    }
}
