using Domain.Interfaces.ApplicationInterfaces;
using IoC;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Services;
using Microsoft.AspNetCore.HttpOverrides;

namespace AtronStock.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAtronCache(Configuration);

            services.AddInfrastructureAPI(Configuration);
            services.AddStockInfrastructure(Configuration);

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
            else
            {
                app.UseHsts();
            }


            AddSwagger(app);


            app.UseReDoc(c =>
            {
                c.RoutePrefix = "docs";
                c.DocumentTitle = "Atron Platform Doc";
                c.SpecUrl = "/swagger/v1/swagger.json";
                c.ExpandResponses("200,201");
            });

            app.UseHttpsRedirection();
            app.UseStatusCodePages();
            app.UseRouting();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static void AddSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron Platform Doc v1"));
        }
    }
}
