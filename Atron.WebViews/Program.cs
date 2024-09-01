using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Atron.WebViews
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureLogging(logging =>
                    {
                        logging.AddConsole();
                    });

                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    // Adiciona o arquivo de configura��o appsettings.json
                    config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

                    // Adiciona tamb�m outras configura��es como vari�veis de ambiente, se necess�rio
                    config.AddEnvironmentVariables();
                });
    }
}