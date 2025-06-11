using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Atron.Infra.IoC
{
    public static class DependencyInjectionCookieContainer
    {
        public static IServiceCollection AddCustomCookieConfiguration(this IServiceCollection services)
        {
            // Configuração do cookie de autenticação
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/";
                options.LoginPath = "/";
                options.LogoutPath = "/";             

                options.Cookie = new CookieBuilder
                {
                    Name = "AuthCookie",
                    HttpOnly = true,
                    SecurePolicy = CookieSecurePolicy.Always,
                    SameSite = SameSiteMode.Strict,
                    IsEssential = true,
                };

            });

            // Configuração da sessão
            services.AddSession(options =>
            {
                options.Cookie.Name = "AuthSession";                            // Nome do cookie
                options.Cookie.HttpOnly = true;                             // Impede acesso via JavaScript
                options.Cookie.IsEssential = true;                         // Necessário para funcionamento essencial
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // HTTPS obrigatório
                options.Cookie.SameSite = SameSiteMode.Strict;           // Restringe envio a origens externas                
            });

            return services;
        }
    }
}