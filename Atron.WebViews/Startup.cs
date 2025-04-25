using Atron.Infra.IoC;
using Atron.WebViews.Helpers;
using Communication.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shared.DTO.API;
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
            services.AddInfrastructure(Configuration);
            services.AddControllersWithViews();
            AppSettings.RotaDeAcesso = Configuration.GetSection(nameof(RotaDeAcesso)).Get<RotaDeAcesso>();
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
            // Middleware para adicionar o token de autenticação no header da requisição
            app.Use(async (context, next) =>
            {
                string token = null;

                // Verifica primeiro se o token está presente nos cookies
                if (context.Request.Cookies.TryGetValue("AuthToken", out var cookieToken))
                {
                    token = cookieToken;
                }
                else
                {
                    // Caso não esteja nos cookies, verifica se o token está na sessão
                    token = context.Session.GetString("AuthToken");
                }

                // Se o token foi encontrado, adiciona no header da requisição
                if (!token.IsNullOrEmpty())
                {
                    context.Request.Headers["Authorization"] = $"Bearer {token}";

                    if (string.IsNullOrEmpty(TokenServiceStore.Token))
                    {
                        TokenServiceStore.Token = token;
                    }
                }

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.Redirect("/ApplicationLogin/Login");
                }

                // Chama o próximo middleware na pipeline
                await next();
            });
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
          
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(

                    name: "default",
                    pattern: "{controller=ApplicationLogin}/{action=Login}");
            });
        }
    }
}