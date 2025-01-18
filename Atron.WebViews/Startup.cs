using Atron.Application.Interfaces;
using Atron.Infra.IoC;
using Atron.WebViews.Delegates;
using Atron.WebViews.Helpers;
using Atron.WebViews.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.DTO.API;
using System;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Shared.Extensions;

namespace Atron.WebViews
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddInfrastructure();
            services.AddCustomCookieConfiguration();
            services.AddInfrastructureSecurity(Configuration);
            services.AddControllersWithViews();
          
            services.Configure<RotaDeAcesso>(Configuration.GetSection(nameof(RotaDeAcesso)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseHttpLogging();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseSession();
            // Middleware para adicionar o token de autentica��o no header da requisi��o
            app.Use(async (context, next) =>
            {
                string token = null;

                // Verifica primeiro se o token est� presente nos cookies
                if (context.Request.Cookies.TryGetValue("AuthToken", out var cookieToken))
                {
                    token = cookieToken;
                }
                else
                {
                    // Caso n�o esteja nos cookies, verifica se o token est� na sess�o
                    token = context.Session.GetString("AuthToken");
                }

                // Se o token foi encontrado, adiciona no header da requisi��o
                if (!token.IsNullOrEmpty())
                {
                    context.Request.Headers["Authorization"] = $"Bearer {token}";
                }

                // Call the next delegate/middleware in the pipeline
                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.AddEntityRoutes();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=ApplicationLogin}/{action=Login}");
            });
        }
    }   
}