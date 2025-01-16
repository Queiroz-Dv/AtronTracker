using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Infra.IoC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Atron.WebApi
{
    /* Classe principal onde é definido as configurações e serviços da API */
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Aqui registramos os serviços da API como repositorios e Validações
            services.AddInfrastructureAPI(Configuration);
            services.AddInfrastructureSecurity(Configuration);
            services.AddDependencyInjectionApiDoc();

            // Indica que usaremos as controllers para comunicação com os endpoints
            services.AddControllers();
            services.AddHttpClient();
            services.AddHttpContextAccessor();

          
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICreateDefaultUserRoleRepository createDefaultUserRole)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();                
            }

            AddSwagger(app);
            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();

            createDefaultUserRole.CreateDefaultRoles();
            createDefaultUserRole.CreateDefaultUsers();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron.WebApi v1"));
        }
    }
}
