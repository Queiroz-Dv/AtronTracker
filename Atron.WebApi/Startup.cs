using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Infra.IoC;
using Atron.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Atron.WebApi
{
    /// <summary>
    /// Classe principal onde é definido as configurações e serviços da API
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Configures the services for the API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Aqui registramos os serviços da API
            services.AddInfrastructureAPI(Configuration);         // Adiciona a injeção de dependência da camada de infraestrutura
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicModuloPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, ModuloHandler>();
            // Indica que usaremos as controllers para comunicação com os endpoints
            services.AddControllers();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c => c.OperationFilter<SwaggerResponseFilter>());
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">The application builder.</param>
        /// <param name="env">The web host environment.</param>
        /// <param name="createDefaultUserRole">The default user role creator.</param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ICreateDefaultUserRoleRepository createDefaultUserRole)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            AddSwagger(app);
            app.UseReDoc(c =>
             {
                 c.RoutePrefix = "docs";
                 c.DocumentTitle = "Atron WebApi Doc";
                 c.SpecUrl = "/swagger/v1/swagger.json";
                 c.ExpandResponses("200,201");
             });
            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();

            createDefaultUserRole.CreateDefaultRoles();
            createDefaultUserRole.CreateDefaultUsers();
            
            app.UseCors("CorsPolicy");
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
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron WebApi Doc v1"));
        }
    }
}