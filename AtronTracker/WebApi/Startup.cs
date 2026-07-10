using IoC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebApi.Helpers;

namespace WebApi
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
            services.AddSingleton<IAuthorizationPolicyProvider, DynamicModuloPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, ModuloHandler>();
            services.AddControllers();
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddSwaggerGen(c => c.OperationFilter<SwaggerResponseFilter>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atron WebApi Doc v1"));
        }
    }
}
