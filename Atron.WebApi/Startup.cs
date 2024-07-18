using Atron.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace Atron.WebApi
{
    /* Classe principal onde � definido as configura��es e servi�os da API */
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Aqui registramos os servi�os da API como repositorios e Valida��es
            services.AddInfrastructureAPI(Configuration);

            // Indica que usaremos as controllers para comunica��o com os endpoints
            services.AddControllers();

            // Informa que usaremos o Swagger para documenta��o e testes
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Atron API",
                    Version = "v1",
                    Description = "Uma API desenvolvida por E. Queiroz para estudos e testes",
                    Contact = new OpenApiContact() { Name = "Eduardo Queiroz", Email = "queiroz.dv@outlook.com" }
                });
                c.EnableAnnotations();                
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron.WebApi v1"));
            }

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
